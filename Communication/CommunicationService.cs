using Communication.Configurator;
using Entities;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace Communication
{
    /// <summary>
    /// Сервис коммуникации между сервером и клиентом
    /// </summary>
    /// <typeparam name="TConfiguration"></typeparam>
    public class CommunicationService<TConfiguration>
        where TConfiguration : CommunicationConfiguratorBase
    {
        private readonly HubConnection hubConnection;
        public CommunicationService(
            string hubUrl,
            TConfiguration configuration,
            CookieContainer? cookieContainer = null)
        {
            Messages = configuration;
            hubConnection = configuration.Configure(hubUrl, cookieContainer);
            hubConnection.Closed += HubConnection_Closed;
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task HubConnection_Reconnecting(Exception? arg)
        {
            Console.WriteLine("Переподключение...");
            Console.WriteLine(arg?.ToString());
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task HubConnection_Reconnected(string? arg)
        {
            Console.WriteLine("Успешно переподключено");
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task HubConnection_Closed(Exception? arg)
        {
            Console.WriteLine("Соединение закрыто");
            return Task.CompletedTask;
        }
        /// <summary>
        /// Набор сообщений в зависимости от конфигурации клиент/сервер
        /// </summary>
        public TConfiguration Messages { get; set; }
        /// <summary>
        /// Выполнить подключение
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            using (CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                await hubConnection.StartAsync(cancellationTokenSource.Token)
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            Console.WriteLine("Подключение успешно выполнено.");
                        }
                    });
        }
        /// <summary>
        /// Зарегистрировать подключение 
        /// как подключение от производства
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public Task RegisterAsClientFactoryAsync(Factory factory)
            => hubConnection.SendAsync("RegisterUserAsFactoryClient", 
                new Message<Factory>(factory, Guid.NewGuid(), string.Empty));
        /// <summary>
        /// Остановить соединение
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            using (CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                return hubConnection.StopAsync(cancellationTokenSource.Token);
        }
    }
}