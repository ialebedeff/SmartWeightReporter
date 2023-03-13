using Communication.Configurator;
using Entities;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace Communication.Client
{
    /// <summary>
    /// Конфигурация подключения для клиента
    /// </summary>
    public class ClientConfiguration : CommunicationConfiguratorBase
    {
        public DatabaseMessageExecutor? DatabaseMessageExecutor { get; set; }
        public override HubConnection Configure(string url, CookieContainer? cookieContainer = null)
        {
            var connectionBuilder = new HubConnectionBuilder()
                 .WithAutomaticReconnect()
                 .WithUrl(url, options => {
                     if (cookieContainer is not null)
                     {
                         options.Cookies = cookieContainer;
                     }
                 });

            var connection = connectionBuilder.Build();

            DatabaseMessageExecutor = new DatabaseMessageExecutor(connection);

            connection.On<Message<DatabaseCommand>>("DatabaseMessageExecute", message =>
            {
                DatabaseMessageExecutor.ExecuteResultAsync(message);
            });

            return connection;
        }

        public override HubConnection Configure(string url, Action<HubConnection> configuration)
        {
            var connection = Configure(url);

            DatabaseMessageExecutor = new DatabaseMessageExecutor(connection);

            configuration.Invoke(connection);

            return connection;
        }
    }
}