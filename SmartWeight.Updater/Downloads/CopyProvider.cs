using Constants.Client;
using Entities;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class CopyProvider : UpdateProvider
    {
        public override async Task ExecuteAsync(
            Build build, Action<ProgressState>? onProgressChanged = null)
        {
            var service = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == Programs.SmartWeightServer);

            if (service is null)
                throw new UpdateException($"Сервис {Programs.SmartWeightServer} не найден.", UpdateState.Copy);

            var downloadedBuildDirectory = Path.Combine(Folders.Builds, Convert.ToString(build.Id));
            var buildDirectory = GetServicePath(service.ServiceName);
            var downloadedFiles = Directory.GetFiles(downloadedBuildDirectory, "*", SearchOption.AllDirectories);

            foreach (var binary in build.Binaries)
            {
                onProgressChanged?.Invoke(new ProgressState(binary.FileName, $"Копирование файла: {binary.FileName}"));

                var directoryPath = Path.Combine(Path.GetDirectoryName(buildDirectory), Path.GetDirectoryName(binary.RelativePath));

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                File.Copy(downloadedFiles.First(df => Path.GetRelativePath(downloadedBuildDirectory, df) == binary.RelativePath), Path.Combine(directoryPath, binary.FileName), true);
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
}
