using Constants.Server;
using Entities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace SmartWeight.RemoteStorage
{
    public class RemoteStorageProvider
    {
        private readonly RemoteBuilds _remoteBuilds;

        /// <summary>
        /// Класс для получения данных с файлового сервера
        /// </summary>
        /// <param name="server">IP адресс сервера. Пример: \\192.168.0.19</param>
        /// <param name="pathToScada">путь до скады. Пример: tts2tb\Sync\Объекты\Автовесы dev\SCADA</param>
        public RemoteStorageProvider(RemoteBuilds remoteBuilds)
        {
            _remoteBuilds = remoteBuilds;
        }

        public int GetLastBuildNumber()
        {
            var builds = Directory.GetDirectories(_remoteBuilds.BuildsFullPath);

            return builds
                .Select(build => Convert.ToInt32(build))
                .OrderByDescending(build => build)
                .FirstOrDefault();
        }

        public IEnumerable<int> GetBuildsNumbers()
        { 
            var builds = Directory
                .GetDirectories(Path.Combine(_remoteBuilds.BuildsFullPath, "dev"))
                .Select(build => Convert.ToInt32(
                    Path.GetFileName(build)));

            return builds;
        }

        public void CopyToLocalRepository(int buildNumber)
        {
            var buildPath = Path.Combine(_remoteBuilds.BuildsFullPath, "dev", Convert.ToString(buildNumber), "bin", "build");
            var buildsPath = Path.Combine(Folders.Builds, Convert.ToString(buildNumber));

            CopyFilesRecursively(buildPath, Path.Combine(Environment.CurrentDirectory, buildsPath));
        }

        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        public Task<byte[]> GetFileAsync(string? absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath) ||
               !File.Exists(absolutePath))
            {
                return Task.FromResult(new byte[0]);
            }

            return File.ReadAllBytesAsync(absolutePath);
        } 

        public async Task<IEnumerable<BinaryFileInformation>?> GetBuildInformationAsync(int buildNumber) 
        {
            var buildPath = Path.Combine(_remoteBuilds.BuildsFullPath, "dev", Convert.ToString(buildNumber), "bin", "build");

            if (!Path.Exists(buildPath))
            {
                return null;
            }

            var buildInformation = new List<BinaryFileInformation>();
            var files = Directory.GetFiles(buildPath, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                buildInformation.Add(new BinaryFileInformation()
                {
                    AbsolutePath = file,
                    FileName = Path.GetFileName(file),
                    RelativePath = Path.GetRelativePath(buildPath, file),
                    Version = FileVersionInfo.GetVersionInfo(file).FileVersion,
                    Hash = Convert.ToBase64String(await GetFileHashAsync(file))
                });
            }

            return buildInformation;
        }

        public async Task<IEnumerable<BuildFileData>> GetBuildFilesAsync(IEnumerable<BinaryFileInformation> files)
        {
            var buildFiles = new List<BuildFileData>();

            foreach (var file in files)
            {
                if (!File.Exists(file.AbsolutePath))
                    continue;

                var buildFile = new BuildFileData()
                {
                    RelativePath = file.RelativePath,
                    Data = await File.ReadAllBytesAsync(file.AbsolutePath)
                };

                buildFiles.Add(buildFile);  
            }

            return buildFiles;
        }

        private async Task<byte[]> GetFileHashAsync(string filePath)
        {
            using (var hash = SHA256.Create())
            using (var stream = new FileStream(filePath, FileMode.Open))
                return await hash.ComputeHashAsync(stream);
        }
    }
}