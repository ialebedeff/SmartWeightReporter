using Constants.Client;
using Entities;
using SmartWeight.Updater.API;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    /// <summary>
    /// Класс ответственный за запуск сервисов после 
    /// конечно установки всех файлов
    /// </summary>
    public class CompletionProvider : UpdateProvider
    {
        private readonly SmartWeightApi _updaterApi;
        public CompletionProvider(SmartWeightApi updaterApi)
        {
            _updaterApi = updaterApi;
        }
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            // Получение списка всех серверов
            ServiceController[] controller = ServiceController.GetServices();

            // Запуск сервиса SmartWeight_server
            foreach (var server in controller.Where(s => s.ServiceName == Programs.SmartWeightServer))
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Запуск сервиса: {server.ServiceName}"));
                await _updaterApi.Service.Process.RunServiceAsync(server.ServiceName);
            }

            // Запуск сервиса mysql_server
            foreach (var mysql in controller.Where(s => s.ServiceName == "mysql_server"))
            {
                onProgressChanged?.Invoke(new ProgressState(null, $"Запуск сервиса: {mysql.ServiceName}"));
                await _updaterApi.Service.Process.RunServiceAsync(mysql.ServiceName);
            }
        }
    }
}
