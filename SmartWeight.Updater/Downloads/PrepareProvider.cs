using Constants.Client;
using Entities;
using SmartWeight.Updater.API;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class PrepareProvider : UpdateProvider
    {
        private readonly SmartWeightApi _updaterApi;
        public PrepareProvider(SmartWeightApi updaterApi)
        {
            _updaterApi = updaterApi;
        }
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            ServiceController[] controller = ServiceController.GetServices();
            Process[] processes = Process.GetProcesses();

            var reporters = processes.Where(p => p.ProcessName == Programs.SmartWeightReporter);
            var backupers = processes.Where(p => p.ProcessName == Programs.SmartWeightReporter);
            var smartWeights = processes.Where(p => p.ProcessName == Programs.SmartWeightReporter);
            var configurators = processes.Where(p => p.ProcessName == Programs.SmartWeightReporter);
            var importers = processes.Where(p => p.ProcessName == Programs.SmartWeightReporter);

            foreach (var reporter in reporters)
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие процесса: {reporter.ProcessName}"));
                await _updaterApi.Service.Process.KillProccessAsync(reporter.Id);
            }

            foreach (var backuper in backupers)
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие процесса: {backuper.ProcessName}"));
                await _updaterApi.Service.Process.KillProccessAsync(backuper.Id);
            }

            foreach (var smartWeigh in smartWeights)
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие процесса: {smartWeigh.ProcessName}"));
                await _updaterApi.Service.Process.KillProccessAsync(smartWeigh.Id);
            }

            foreach (var configurator in configurators)
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие процесса: {configurator.ProcessName}"));
                await _updaterApi.Service.Process.KillProccessAsync(configurator.Id);
            }

            foreach (var importer in importers)
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие процесса: {importer.ProcessName}"));
                await _updaterApi.Service.Process.KillProccessAsync(importer.Id);
            }

            foreach (var server in controller.Where(s => s.ServiceName == "SmartWeight_server"))
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие сервиса: {server.ServiceName}"));
                await _updaterApi.Service.Process.StopServiceAsync(server.ServiceName);
            }
            foreach (var mysql in controller.Where(s => s.ServiceName == "mysql_server"))
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Закрытие сервиса: {mysql.ServiceName}"));
                await _updaterApi.Service.Process.StopServiceAsync(mysql.ServiceName);
            }
        }
    }
}
