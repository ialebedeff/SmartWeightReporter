using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Updater.API;
using System.Reactive;

namespace SmartWeight.Panel.Client.Pages.Login;
public class ViewModelBase : ReactiveObject
{
    public ViewModelBase(
        ApplicationState applicationState,
        ISnackbar snackbar,
        IDialogService dialog,
        SmartWeightApi updaterApi,
        NavigationManager navigation,
        CommunicationService<ServerConfiguration> communicationService,
        DatabaseMessageFactory databaseMessageFactory)
    {
        ApiClient = updaterApi;
        Snackbar = snackbar;
        Dialog = dialog;
        Navigation = navigation;
        CommunicationService = communicationService;
        DatabaseMessageFactory = databaseMessageFactory;
        ApplicationState = applicationState;
        ConnectionCommand = ReactiveCommand.CreateFromTask(CommunicationService.StartAsync);
        ConnectionCommand.ThrownExceptions.Subscribe(OnConnectionFailure);
    }
    public SmartWeightApi ApiClient { get; set; } = null!;
    public ISnackbar Snackbar { get; set; } = null!;
    public NavigationManager Navigation { get; set; } = null!;
    public IDialogService Dialog { get; set; } = null!;
    public CommunicationService<ServerConfiguration> CommunicationService { get; set; } = null!;
    public DatabaseMessageFactory DatabaseMessageFactory { get; set; }
    public ReactiveCommand<Unit, Unit> ConnectionCommand { get; set; }
    public ApplicationState ApplicationState { get; set; }
    private void OnConnectionFailure(Exception exception)
    {
        this.Snackbar.Add(exception.Message);
    }
}
