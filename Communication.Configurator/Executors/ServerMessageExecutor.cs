﻿using DynamicData.Binding;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace Communication.Configurator
{
    public abstract class ServerMessageExecutor<TParam, TResult> : MessageExecutor<TParam, TResult>
    {
        [AllowNull]
        private ObservableCollectionExtended<TResult> _results;
        public ObservableCollectionExtended<TResult> Results
        {
            get => _results;
            set => this.RaiseAndSetIfChanged(ref _results, value);
        }
        protected ServerMessageExecutor(HubConnection hubConnection, string methodName)
            : base(hubConnection, methodName)
        {
            Results = new ObservableCollectionExtended<TResult>();
        }
        public abstract Task<IEnumerable<TResult>> ExecuteToResultsAsync(Message<TParam> message);
        public virtual Task SendMessageAsync<TData>(Message<TData> message)
            where TData : class
        {
            using (CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                return HubConnection.SendAsync(MethodName, message, cancellationTokenSource.Token);
        }
        public virtual Task SendMessageAsync()
        {
            using (CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                return HubConnection.SendAsync(MethodName, cancellationTokenSource.Token);
        }
        public void InsertResult(TResult param)
            => Results = new ObservableCollectionExtended<TResult>() { param };
    }
}