using Communication.Configurator;
using Entities.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Communication.Server;

public class ConnectionStateMessageExecutor<T> : ServerMessageExecutor<ConnectionStateChanged, T>
{
    public ConnectionStateMessageExecutor(HubConnection connection) : base(connection, string.Empty)
    { 
    }

    public override Task<T> ExecuteAsync(Message<ConnectionStateChanged> message)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<T>> ExecuteToResultsAsync(Message<ConnectionStateChanged> message)
    {
        throw new NotImplementedException();
    }
}
