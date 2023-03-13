using Communication;
using Communication.Configurator;
using Communication.Server;
using DynamicData.Binding;
using Entities.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Data;
using System.Reactive.Disposables;

namespace SmartWeight.Panel.Client.Shared.ViewModels
{
    public class AuthorizedDashboardViewModel : ViewModelBase, IActivatableViewModel
    {
        public AuthorizedDashboardViewModel(ApplicationState applicationState
            , ISnackbar snackbar
            , IDialogService dialog
            , RestApiClients updaterApi
            , NavigationManager navigation
            , CommunicationService<ServerConfiguration> communicationService
            , DatabaseMessageFactory databaseMessageFactory) 
            : base(applicationState
                  , snackbar
                  , dialog
                  , updaterApi
                  , navigation
                  , communicationService
                  , databaseMessageFactory)
        {
            ConnectionCommand.Execute();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.CommunicationService.Messages.UserConnectionState.ConnectionStates.Results)
                    .Subscribe(results => NotifyOnConnectionClientsChanged(results))
                    .DisposeWith(disposables);
            });
        }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new ViewModelActivator();
        /// <summary>
        /// Оповещение об отключении / подключении сервиса клиента
        /// </summary>
        /// <param name="connectionStateChangeds"></param>
        private void NotifyOnConnectionClientsChanged(ObservableCollectionExtended<ConnectionStateChanged> connectionStateChangeds)
        {
            if (connectionStateChangeds is null) return;
            if (connectionStateChangeds.Count is 0) return;

            var connectionState = connectionStateChangeds.First();

            if (connectionState.State is ConnectionState.Open &&
                connectionState.HubClient.IsFactoryUser)
            {
                Snackbar.Add($"Пользователь: {connectionState.HubClient.Factory?.Title} подключен", severity: Severity.Info);
                if(!ApplicationState.ConnectedClients.Any(client => client.Factory?.Id == connectionState.HubClient.Factory?.Id))
                    ApplicationState.ConnectedClients.Add(connectionState.HubClient);
            }
            else if (connectionState.State is ConnectionState.Closed &&
                     connectionState.HubClient.IsFactoryUser)
            {
                Snackbar.Add($"Пользователь: {connectionState.HubClient.Factory?.Title} отключился", severity: Severity.Error);
                ApplicationState.ConnectedClients.Remove(connectionState.HubClient);
            }
        }
    }
}
