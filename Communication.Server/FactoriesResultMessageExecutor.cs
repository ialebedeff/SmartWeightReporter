using Communication.Configurator;
using Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace Communication.Server;

public class FactoriesResultMessageExecutor<T> : ServerMessageExecutor<Factory, T>
{
    public FactoriesResultMessageExecutor(HubConnection hubConnection) : base(hubConnection, "GetConnectedUsers")
    {
    }

    public override Task<T> ExecuteAsync(Message<Factory> message)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<T>> ExecuteToResultsAsync(Message<Factory> message)
    {
        throw new NotImplementedException();
    }
}