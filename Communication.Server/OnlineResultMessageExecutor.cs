using Communication.Configurator;
using Microsoft.AspNetCore.SignalR.Client;

namespace Communication.Server;

public class OnlineResultMessageExecutor<T> : ServerMessageExecutor<bool, T>
{
    public OnlineResultMessageExecutor(HubConnection hubConnection) : base(hubConnection, "DatabaseMessageExecute")
    {
    }

    public override Task<T> ExecuteAsync(Message<bool> message)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<T>> ExecuteToResultsAsync(Message<bool> message)
    {
        throw new NotImplementedException();
    }
}
