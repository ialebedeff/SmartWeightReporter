using Constants.Client;
using Entities;
using SmartWeight.Updater.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class DownloadProvider : UpdateProvider
    {
        private Action<ProgressState>? _onProgressChanged;
        private readonly SmartWeightApi _clientApi;
        public DownloadProvider(SmartWeightApi smartWeightApi)
        {
            _clientApi = smartWeightApi;
        }

        public override Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            _onProgressChanged = onProgressChanged;

            string buildDirectory = Path.Combine(Folders.Builds, Convert.ToString(build.Id));

            if (CreateDirectoryIfNotExists(buildDirectory))
            {
                return DownloadBuildAsync(buildDirectory, build);
            }
            else
            {
                return DownloadExceptBuildAsync(buildDirectory, build);
            }
        }
        private bool CreateDirectoryIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                return true;
            }

            return false;
        }
        private Task DownloadBuildAsync(string buildDirectory, Build build)
           => DownloadFilesAsync(build.Id, buildDirectory, build.Binaries);

        private async Task DownloadExceptBuildAsync(string buildDirectory, Build build)
        {
            var completedFiles = Directory.GetFiles(buildDirectory, "*", SearchOption.AllDirectories);
            var completedFilesHashes = await HashHelper.GetFilesHashesAsync(completedFiles);
            var uncompletedFiles = build.Binaries.Where(file => !completedFilesHashes.Contains(file.Hash));

            await DownloadFilesAsync(build.Id, buildDirectory, uncompletedFiles);
        }

        private async Task DownloadFilesAsync(int buildId, string buildDirectory, IEnumerable<BinaryFileInformation> binaryFiles)
        {
            int totalDownloaded = 0;
            int totalFiles = binaryFiles.Count();

            foreach (var binary in binaryFiles)
            {
                await _clientApi.Server.Update
                    .GetBuildFileAsync(buildId, binary.Id)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted) throw new UpdateException("Произошла ошибка при загруке некоторых файлов.", UpdateState.Download);

                        if (task.IsCompletedSuccessfully)
                        {
                            var filePath = Path.Combine(buildDirectory, binary.RelativePath);
                            var directory = Directory.CreateDirectory(
                                Path.Combine(buildDirectory,
                                binary.RelativePath.Replace(binary.FileName, "")));

                            totalDownloaded += 1;

                            _onProgressChanged?.Invoke(
                                new ProgressState(
                                    totalFiles, 
                                    totalDownloaded, 
                                    binary.FileName, 
                                    $"Загружено файлов: {totalDownloaded} из {totalFiles}"));

                            File.WriteAllBytesAsync(filePath, task.Result);
                        }
                    });

                GC.Collect();
            }
        }
    }
}
