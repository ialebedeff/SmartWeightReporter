using Constants.Client;
using Entities;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    /// <summary>
    /// Класс ответственный за миграции
    /// </summary>
    public class MigrateProvider : UpdateProvider
    {
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            // Получение сервиса SmartWeight_server
            var service = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == Programs.SmartWeightServer);

            if (service is null)
            {
                throw new UpdateException($"Сервис {Programs.SmartWeightServer} не найден", UpdateState.Migrate);
            }

            onProgressChanged?.Invoke(new ProgressState("", "Применяются миграции к базе данных..."));

            // Получение пути до Server.exe
            var serverPath = GetServiceExecutionPath(service.ServiceName);

            // Создается процесс Server.exe --migrate
            // для применения миграций, если миграции
            // не буду применены за 10 сек. выкинет Exception
            var migrationProcess = Process.Start(new ProcessStartInfo(serverPath)
            {
                Arguments = "--migrate"
            });

            // Ожидание до выхода из процесса
            if (migrationProcess is not null)
            {
                using (CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await migrationProcess.WaitForExitAsync(source.Token);
                }
            }
        }
    }

    /// <summary>
    /// Класс ответственный за проверку текущих установленных файлов
    /// </summary>
    public class CheckerProvider : UpdateProvider
    {
        /// <summary>
        /// Сравнить файлы с выбранным билдом
        /// </summary>
        /// <param name="build"></param>
        /// <param name="onProgressChanged"></param>
        /// <returns></returns>
        /// <exception cref="UpdateException"></exception>
        /// <exception cref="Exception"></exception>
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            var service = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == Programs.SmartWeightServer);

            if (service is null)
                throw new UpdateException($"Сервис {Programs.SmartWeightServer} не найден", UpdateState.Check);

            var buildDirectory = GetServicePath(service.ServiceName);
            var files = Directory.GetFiles(buildDirectory, "*", SearchOption.AllDirectories);
            var hashes = await HashHelper.GetFilesHashesAsync(files);
            var bins = build.Binaries
                .Where(binary => !hashes.Contains(binary.Hash));
            //var hash = await HashHelper.GetFileHashAsync("C:\\SmartMix\\x86\\grdspactivate.dll");
            //var tHash = await HashHelper.GetFileHashAsync("C:\\Users\\Public\\Documents\\SmartWeight\\Builds\\86\\x86\\grdspactivate.dll");

            //var Hash64 = await HashHelper.GetFileHashAsync("C:\\SmartMix\\x64\\grdspactivate.dll");
            //var tHash64 = await HashHelper.GetFileHashAsync("C:\\Users\\Public\\Documents\\SmartWeight\\Builds\\86\\x64\\grdspactivate.dll");

            // Если коллекция файлов больше нуля, то есть необновлённые файлы
            if (bins.Count() == 0) throw new Exception("У вас уже установлено последнее обновление");
            //var completedFiles = Directory.GetFiles(buildDirectory, "*", SearchOption.AllDirectories);
            //var completedFilesHashes = await HashHelper.GetFilesHashesAsync(completedFiles, onProgressChanged);
            //var uncorrectFiles = build.Binaries
            //    .Select(binary => binary.Hash)
            //    .Where(hash => !completedFilesHashes.Contains(hash));

            //if (uncorrectFiles.Count() == 0)
            //    throw new Exception("У вас уже установлено текущее обновление");
        }
    }
}
