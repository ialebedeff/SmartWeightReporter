using Communication;
using Communication.Configurator;
using Communication.Server;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Updater.API;
using System.Reactive;

namespace SmartWeight.Panel.Client.Pages.Login;
public class ViewModelBase : ReactiveObject
{
    public ViewModelBase(ApplicationState applicationState
        , ISnackbar snackbar
        , IDialogService dialog
        , RestApiClients updaterApi
        , NavigationManager navigation
        , CommunicationService<ServerConfiguration> communicationService
        , DatabaseMessageFactory databaseMessageFactory)
    {
        ApiClients = updaterApi;
        Snackbar = snackbar;
        Dialog = dialog;
        Navigation = navigation;
        CommunicationService = communicationService;
        DatabaseMessageFactory = databaseMessageFactory;
        ApplicationState = applicationState;
        ConnectionCommand = ReactiveCommand.CreateFromTask(CommunicationService.StartAsync);
        ConnectionCommand.ThrownExceptions.Subscribe(OnConnectionFailure);
    }
    /// <summary>
    /// API`s 
    /// </summary>
    public RestApiClients ApiClients { get; set; } = null!;
    /// <summary>
    /// Снэкбар, используется для 
    /// вывода пользовательских ошибок
    /// </summary>
    public ISnackbar Snackbar { get; set; } = null!;
    /// <summary>
    /// Менеджер для навигаций
    /// </summary>
    public NavigationManager Navigation { get; set; } = null!;
    /// <summary>
    /// Менеджер диалоговых окон
    /// </summary>
    public IDialogService Dialog { get; set; } = null!;
    /// <summary>
    /// Сервис для коммуникации с сервером -> клиентом
    /// </summary>
    public CommunicationService<ServerConfiguration> CommunicationService { get; set; } = null!;
    /// <summary>
    /// Фабрика для создания команд для БД
    /// </summary>
    public DatabaseMessageFactory DatabaseMessageFactory { get; set; }
    /// <summary>
    /// Команда для подключения к серверу
    /// </summary>
    public ReactiveCommand<Unit, Unit> ConnectionCommand { get; set; }
    /// <summary>
    /// "Кэш" приложения, используется для того 
    /// чтобы хранить какие-то пользовательские данные
    /// </summary>
    public ApplicationState ApplicationState { get; set; }
    /// <summary>
    /// Вывод ошибки при невозможности подключения к серверу
    /// </summary>
    /// <param name="exception"></param>
    private void OnConnectionFailure(Exception exception)
    {
        this.Snackbar.Add(exception.Message);
    }
}
