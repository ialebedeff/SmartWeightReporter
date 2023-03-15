using Communication;
using Communication.Configurator;
using Communication.Server;
using DynamicData.Binding;
using Entities;
using Entities.Database;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Reactive;
using System.Reactive.Disposables;

namespace SmartWeight.Panel.Client.Pages.WeighingsData
{
    public class FinalWeighingsViewModel : FilterableViewModel, IActivatableViewModel
    {
        public FinalWeighingsViewModel(
              Filter filter
            , ApplicationState applicationState
            , ISnackbar snackbar
            , IDialogService dialog
            , RestApiClients updaterApi
            , NavigationManager navigation
            , CommunicationService<ServerConfiguration> communicationService
            , DatabaseMessageFactory databaseMessageFactory) 
            : base(filter
                  , applicationState
                  , snackbar
                  , dialog
                  , updaterApi
                  , navigation
                  , communicationService
                  , databaseMessageFactory)
        {
            Activator = new ViewModelActivator();
            LoadWeighingsCommand = ReactiveCommand.CreateFromTask(LoadDataByFilterAsync);
            LoadCurrentFactoryCommand = ReactiveCommand.CreateFromTask(LoadCurrentFactoryAsync);

            this.WhenActivated(disposables =>
            {
                var factoryErrorSubscription = LoadCurrentFactoryCommand.ThrownExceptions
                        .Subscribe(OnLoadFailure)
                        .DisposeWith(disposables);

                var weighingsErrorSubscription = LoadWeighingsCommand.ThrownExceptions
                        .Subscribe(OnLoadFailure)
                        .DisposeWith(disposables);

                var loadWeighingsSubscription = this.WhenAnyValue(x => x.Factory)
                        .Subscribe(_ => LoadWeighingsCommand.Execute())
                        .DisposeWith(disposables);

                var weighingsResultObservable = this
                        .WhenAnyValue(x => x.CommunicationService.Messages.DatabaseHub.Weighings.Results)
                        .Subscribe(results => OnResultsChanged(results))
                        .DisposeWith(disposables);
            });
        }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        /// <summary>
        /// Загрузить текущее производство
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadCurrentFactoryCommand { get; set; }
        /// <summary>
        /// Загрузить информацию по взвешиваниям
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadWeighingsCommand { get; set; }
        /// <summary>
        /// Производство
        /// </summary>
        private Factory? _factory;
        /// <summary>
        /// Производство
        /// </summary>
        public Factory? Factory
        {
            get { return _factory; }
            set { this.RaiseAndSetIfChanged(ref _factory, value); }
        }
        /// <summary>
        /// Информация по взвешивания
        /// </summary>
        private ObservableCollectionExtended<Weighings> _weighings = new();
        /// <summary>
        /// Информация по звешиваниям
        /// </summary>
        public ObservableCollectionExtended<Weighings> Weighings
        {
            get { return _weighings; }
            set { this.RaiseAndSetIfChanged(ref _weighings, value); }
        }
        /// <summary>
        /// Загрузка текущего производства
        /// </summary>
        /// <returns></returns>
        public async Task LoadCurrentFactoryAsync()
        {
            ApplicationState.CurrentFactory = await ApiClients.Server.Factory.GetCurrentFactoryAsync();
            Factory = ApplicationState.CurrentFactory;
        }
        /// <summary>
        /// Вывод ошибки при загрузке
        /// </summary>
        /// <param name="exception"></param>
        public void OnLoadFailure(Exception exception)
        {
            Snackbar.Add(exception.Message);
        }
        /// <summary>
        /// Загрузить информацию по фильру
        /// </summary>
        /// <returns></returns>
        public override async Task LoadDataByFilterAsync()
        {
            if (Factory is not null)
            {
                var message = DatabaseMessageFactory.CreateSelectWeighingsCommand(Factory, Factory.DatabaseConnection, Filter);
                await CommunicationService.Messages.DatabaseHub.Weighings.SendMessageAsync(message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weighings"></param>
        private void OnResultsChanged(ObservableCollectionExtended<Weighings> weighings)
        {
            Weighings.Clear();
            Weighings.AddRange(weighings);
        }
    }
}
