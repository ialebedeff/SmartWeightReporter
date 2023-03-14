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
using System.Reactive.Disposables;

namespace SmartWeight.Panel.Client.Components
{
    /// <summary>
    /// Вью модель фильтра
    /// </summary>
    public class FilterViewModel : ViewModelBase, IActivatableViewModel
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
            LoadUserFactoriesCommand = ReactiveCommand.CreateFromTask(_ => ApiClients.Server.Factory.GetCurrentUserFactoriesAsync());

            this.WhenActivated(disposables =>
            {
                LoadUserFactoriesCommand
                    .Execute()
                    .Subscribe(factories => UserFactories = factories)
                    .DisposeWith(disposables);
            });
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
        /// Команда для загрузки производств пользователя
        /// </summary>
        public ReactiveCommand<Unit, IEnumerable<Factory>?> LoadUserFactoriesCommand { get; set; }
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
        /// <summary>
        /// Производства пользователя
        /// </summary>
        private IEnumerable<Factory>? _userFactories;
        /// <summary>
        /// Производства пользователя
        /// </summary>
        public IEnumerable<Factory>? UserFactories
        {
            get { return this._userFactories; }
            set { this.RaiseAndSetIfChanged(ref _userFactories, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Factory>? SelectedFactories
        {
            get { return this.ApplicationState.SelectedFactories; }
            set 
            {
                if (value is not null)
                {
                    ApplicationState.SelectedFactories = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
    }
}
