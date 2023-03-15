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
        public DatabaseExecutor? WeighingsExecutor { get; set; }
        public DatabaseExecutor? WorkCarsExecutor { get; set; }
        public WeighingsDatabaseExecutor? WeighingsDatabaseExecutor { get; set; }
        public TruckDatabaseExecutor? TruckDatabaseExecutor { get; set; }
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

            TruckDatabaseExecutor = new TruckDatabaseExecutor(connection, "CarsDatabaseMessageResult");
            WeighingsDatabaseExecutor = new WeighingsDatabaseExecutor(connection, "DatabaseMessageResult");

            connection.On<Message<DatabaseCommand>>("DatabaseMessageExecute", message =>
            {
                Console.WriteLine("{0} сообщение: {1}", "DatabaseMessageExecute", JsonSerializer.Serialize(message));
                WeighingsDatabaseExecutor.ExecuteAndSendResultAsync(message);
            });
            connection.On<Message<DatabaseCommand>>("CarsDatabaseMessageExecute", message =>
            {
                Console.WriteLine("{0} сообщение: {1}", "CarsDatabaseMessageExecute", JsonSerializer.Serialize(message));
                TruckDatabaseExecutor.ExecuteAndSendResultAsync(message);
            });

            return connection;
        }

        public override HubConnection Configure(string url, Action<HubConnection> configuration)
        {
            var connection = Configure(url);

            WeighingsExecutor = new DatabaseExecutor(connection, "");

            configuration.Invoke(connection);

            return connection;
        }
    }
}