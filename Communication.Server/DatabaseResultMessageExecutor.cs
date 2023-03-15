using Communication.Configurator;
using Microsoft.AspNetCore.SignalR.Client;

namespace Communication.Server;

public class DatabaseResultMessageExecutor<T> : ServerMessageExecutor<IEnumerable<Dictionary<string, object>>, T>
    where T : class
{
    public DatabaseResultMessageExecutor(HubConnection hubConnection, string methodName) : base(hubConnection, methodName)
    {
    }
    public override Task<T> ExecuteAsync(Message<IEnumerable<Dictionary<string, object>>> message)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<T>> ExecuteToResultsAsync(Message<IEnumerable<Dictionary<string, object>>> message)
    {
        return Task.FromResult(AutoConverter.ConvertToEnumerableOfTypes<T>(message.Data));
    }
}
