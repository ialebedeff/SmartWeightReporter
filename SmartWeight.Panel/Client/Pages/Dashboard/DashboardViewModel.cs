using Communication;
using Communication.Configurator;
using Communication.Server;
using DynamicData;
using DynamicData.Binding;
using Entities;
using Entities.Database;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Reactive;
using System.Reactive.Linq;
using System.Xml.Schema;

namespace SmartWeight.Panel.Client.Pages.Dashboard;

public class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel(ChartJS chartJS,
        ApplicationState applicationState,
        ISnackbar snackbar,
        IDialogService dialog, 
        RestApiClients updaterApi, 
        NavigationManager navigation, 
        CommunicationService<ServerConfiguration> communicationService, 
        DatabaseMessageFactory databaseMessageFactory) 
        : base(applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
    {
        
    }

}
