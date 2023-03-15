using Communication.Configurator;
using Entities;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using System.Text.Json;

namespace Communication.Client
{
    /// <summary>
    /// Конфигурация подключения для клиента
    /// </summary>
    public class ClientConfiguration : CommunicationConfiguratorBase
    {
        public DatabaseMessageExecutor? WeighingsExecutor { get; set; }
        public DatabaseMessageExecutor? WorkCarsExecutor { get; set; }
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

            WeighingsExecutor = new DatabaseMessageExecutor(connection, "DatabaseMessageResult");
            WorkCarsExecutor= new DatabaseMessageExecutor(connection, "CarsDatabaseMessageResult");

            connection.On<Message<DatabaseCommand>>("DatabaseMessageExecute", message =>
            {
                Console.WriteLine("{0} сообщение: {1}", "DatabaseMessageExecute", JsonSerializer.Serialize(message));
                WeighingsExecutor.ExecuteResultAsync(message);
            });
            connection.On<Message<DatabaseCommand>>("CarsDatabaseMessageExecute", message =>
            {
                Console.WriteLine("{0} сообщение: {1}", "CarsDatabaseMessageExecute", JsonSerializer.Serialize(message));
                WorkCarsExecutor.ExecuteResultAsync(message);
            });

            return connection;
        }

        public override HubConnection Configure(string url, Action<HubConnection> configuration)
        {
            var connection = Configure(url);

            WeighingsExecutor = new DatabaseMessageExecutor(connection, "");

            configuration.Invoke(connection);

            return connection;
        }
    }
}