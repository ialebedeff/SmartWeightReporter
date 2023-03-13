using Constants.Client;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{

    public class ApplicationBackupProvider : UpdateProvider
    {
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            var service = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == Programs.SmartWeightServer);

            if (service is null)
                throw new UpdateException($"Сервис {Programs.SmartWeightServer} не найден.", UpdateState.Copy);

            var backupFolderName = DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss");
            var currentPath = GetServicePath(service.ServiceName);
            var currentFiles = Directory.GetFiles(currentPath, "*", SearchOption.AllDirectories);
            var backupDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Backups", "Application", "SmartWeight", backupFolderName);

            foreach (var currentFile in currentFiles)
            {
                onProgressChanged?.Invoke(new ProgressState(currentFile, $"Бэкап: {currentFile}"));

                var relativePath = Path.GetRelativePath(currentPath, currentFile);
                var relativeDir = Path.Combine(backupDirectoryPath, relativePath);
                var rel = Path.GetRelativePath(relativePath, relativeDir);

                if (!Directory.Exists(Path.GetDirectoryName(rel)))
                    Directory.CreateDirectory(Path.GetDirectoryName(rel));
                
                await Task.Delay(5);

                File.Copy(currentFile, rel, true);
            }
        }

        private string? GetServicePath(string serviceName)
        {
            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + serviceName + "'"))
            {
                wmiService.Get();
                return (wmiService["PathName"]?
                    .ToString())?
                    .Replace("Server.exe", "")
                    .Replace("\"", "");
            }
        }
    }

    public class ValidationProvider : UpdateProvider
    {
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            var buildDirectory = Path.Combine(Folders.Builds, Convert.ToString(build.Id));
            var completedFiles = Directory.GetFiles(buildDirectory, "*", SearchOption.AllDirectories);
            var completedFilesHashes = await HashHelper.GetFilesHashesAsync(completedFiles, onProgressChanged);
            var uncorrectFiles = build.Binaries
                .Select(binary => binary.Hash)
                .Where(hash => !completedFilesHashes.Contains(hash));

            if (uncorrectFiles.Count() > 0)
                throw new UpdateException(
                    "Некоторые файлы были загружены не правильно.", UpdateState.Validate);
        }
    }
}
