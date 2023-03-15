using Communication.Configurator;
using DynamicData.Binding;
using Entities;
using Entities.Database;
using Entities.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Net;

namespace Communication.Server;
public class DatabaseApi
{
    public DatabaseApi(HubConnection hubConnection)
    {
        Weighings = new DatabaseResultMessageExecutor<Weighings>(hubConnection, "DatabaseMessageExecute");
        Cars = new DatabaseResultMessageExecutor<Truck>(hubConnection, "CarsDatabaseMessageExecute");
    }

    public DatabaseResultMessageExecutor<Truck> Cars { get; set; }
    public DatabaseResultMessageExecutor<Weighings> Weighings { get; set; }
}

public class OnlineApi
{
    public OnlineApi(HubConnection hubConnection)
    {
        Onlines = new OnlineResultMessageExecutor<bool>(hubConnection);
    }

    public OnlineResultMessageExecutor<bool> Onlines { get; set; }
}

public class UserConnectionStateApi
{
    public UserConnectionStateApi(HubConnection hubConnection)
    {
        ConnectionStates = new ConnectionStateMessageExecutor<ConnectionStateChanged>(hubConnection);
    }

    public ConnectionStateMessageExecutor<ConnectionStateChanged> ConnectionStates { get; set; }
}

public class ClientsApi
{
    public ClientsApi(HubConnection hubConnection)
    {
        Clients = new FactoriesResultMessageExecutor<HubClient>(hubConnection);
    }

    public FactoriesResultMessageExecutor<HubClient> Clients { get; set; }
}
public class ServerConfiguration : CommunicationConfiguratorBase
{
    public DatabaseApi DatabaseHub { get; set; } = null!;
    public OnlineApi OnlineHub { get; set; } = null!;
    public ClientsApi ClientsHub { get; set; } = null!;
    public UserConnectionStateApi UserConnectionStateHub { get; set; } = null!;
    public override HubConnection Configure(string url, CookieContainer? cookieContainer = null)
    {
        var connectionBuilder = new HubConnectionBuilder()
             .WithAutomaticReconnect()
             .WithUrl(url);

        var connection = connectionBuilder.Build();

        DatabaseHub = new DatabaseApi(connection);
        OnlineHub = new OnlineApi(connection);
        ClientsHub = new ClientsApi(connection);
        UserConnectionStateHub = new UserConnectionStateApi(connection);

        connection.On<Message<IEnumerable<Weighings>>>("DatabaseMessageResult", message =>
        {
            DatabaseHub.Weighings.Results = new ObservableCollectionExtended<Weighings>(message.Data);
        });
        connection.On<Message<IEnumerable<Truck>>>("CarsDatabaseMessageResult", message =>
        {
            DatabaseHub.Cars.Results = new ObservableCollectionExtended<Truck>(message.Data);
        });
        //UserConnectionStateChanged
        connection.On<Message<ObservableCollection<HubClient>>>("ReceiveConnectedUsers", message =>
        {
            ClientsHub.Clients.Results = new ObservableCollectionExtended<HubClient>(message.Data);
        });
        connection.On<Message<ConnectionStateChanged>>("UserConnectionStateChanged", message =>
        {
            UserConnectionStateHub.ConnectionStates.InsertResult(message.Data);
        });
        return connection;
    }

    public override HubConnection Configure(string url, Action<HubConnection> configuration)
    {
        var connection = Configure(url);

        configuration.Invoke(connection);

        return connection;
    }
}