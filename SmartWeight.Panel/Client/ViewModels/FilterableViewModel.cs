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

public class FilterableViewModel : ViewModelBase
{
    public FilterableViewModel(Filter filter
        , ApplicationState applicationState
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
        Filter = filter;
        LoadDataByFilterCommand = ReactiveCommand.CreateFromTask(LoadDataByFilterAsync);
    }
    /// <summary>
    /// Фильтр
    /// </summary>
    public Filter Filter { get; set; }
    /// <summary>
    /// Загрузить данные по фильтру
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadDataByFilterCommand { get; set; }
    /// <summary>
    /// Очистка филтра
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; set; }
    /// <summary>
    /// Загрузить данные по фильтру
    /// </summary>
    /// <returns></returns>
    public virtual Task LoadDataByFilterAsync()
    {
        return Task.CompletedTask;
    }
}
