using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Reactive;

namespace SmartWeight.Panel.Client.Components
{
    /// <summary>
    /// Вью модель фильтра
    /// </summary>
    public class FilterViewModel : ViewModelBase
    {
        public FilterViewModel(
              Filter filter
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
            ClearFilterCommand = ReactiveCommand.Create(ClearFilter);
        }
        /// <summary>
        /// Фильтр
        /// </summary>
        public Filter Filter { get; set; }
        /// <summary>
        /// Команда для очистки фильтра
        /// </summary>
        public ReactiveCommand<Unit, Unit>? ClearFilterCommand { get; set; }
        /// <summary>
        /// Команда для загрузки данные по фильтру
        /// </summary>
        public ReactiveCommand<Unit, Unit>? ReloadFilterCommand { get; set; }
        /// <summary>
        /// Очистка фильтра
        /// </summary>
        private void ClearFilter()
        {
            Filter.CarNumber = string.Empty;
            Filter.Material = string.Empty;
            Filter.From = null;
            Filter.To = null;
            Filter.Shipper = string.Empty;

            this.RaisePropertyChanged(nameof(Filter));
        }
    }
}
