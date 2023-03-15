using Communication;
using Communication.Configurator;
using Communication.Server;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;

namespace SmartWeight.Panel.Client.Dialogs
{
    public class TruckDetailsViewModel : ViewModelBase
    {
        public TruckDetailsViewModel(ApplicationState applicationState
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
        }
    }
}
