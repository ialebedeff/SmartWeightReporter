using Entities;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;

namespace Communication.Configurator
{
    public abstract class DatabaseMessageExecutor<TOutput> : ReactiveObject
    {
        private readonly string _methodName;
        private readonly HubConnection _hubConnection;
        public DatabaseMessageExecutor(HubConnection hubConnection, string responseUrl)
        { 
            _hubConnection = hubConnection;
            _methodName = responseUrl;
        }

        public abstract Task<IEnumerable<TOutput>> ExecuteCommandAsync(DatabaseCommand databaseCommand);
        public Task ExecuteAndSendResultAsync(Message<DatabaseCommand> message) 
        {
            return ExecuteCommandAsync(message.Data)
                .ContinueWith(task => 
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Message<IEnumerable<TOutput>> response = Message<IEnumerable<TOutput>>.CreateResponse(message, task.Result);
                        using (CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                            _hubConnection.SendAsync(_methodName, response, tokenSource.Token);
                    }
                    else { Console.WriteLine(task.Exception?.Message); }
                });
        }
    }
    public abstract class MessageExecutor<TParam, TResult> : ReactiveObject
    {
        public MessageExecutor(HubConnection hubConnection, string responseUrl)
        {
            HubConnection = hubConnection;
            MethodName = responseUrl;
        }
        public string MethodName { get; set; }
        public HubConnection HubConnection { get; set; }
        public abstract Task<TResult> ExecuteAsync(Message<TParam> message);
        public virtual Task SendAsync(string uri, Message<TParam> message) { return Task.CompletedTask; }
        public virtual Task SendAsync(string uri, Message message) { return Task.CompletedTask; }
        public virtual Task SendAsync(string uri) { return Task.CompletedTask; }
        public virtual Task ExecuteResultAsync(Message<TParam> message)
        {
            return ExecuteAsync(message)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {//CarsDatabaseMessageResult
                        Message<TResult> response = Message<TResult>.CreateResponse(message, task.Result);
                        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                            HubConnection.SendAsync(MethodName, response, cancellationTokenSource.Token);
                    }
                });
        }
    }
}