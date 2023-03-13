using Communication;
using Communication.Configurator;
using Communication.Server;
using DynamicData.Binding;
using Entities;
using Entities.Database;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Components;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Security.Cryptography.X509Certificates;
using static System.Reflection.Metadata.BlobBuilder;

namespace SmartWeight.Panel.Client.Pages.WeighingsData
{
    public class FinalWeighingsViewModel : FilterableViewModel, IActivatableViewModel
    {
        public FinalWeighingsViewModel(
            Filter filter,
            ApplicationState applicationState,
            ISnackbar snackbar,
            IDialogService dialog,
            SmartWeightApi updaterApi,
            NavigationManager navigation,
            CommunicationService<ServerConfiguration> communicationService,
            DatabaseMessageFactory databaseMessageFactory) : base(filter, applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
        {
            Activator = new ViewModelActivator();
            Weighings = new ObservableCollectionExtended<Weighings>();

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
                        .WhenAnyValue(x => x.CommunicationService.Messages.Database.Weighings.Results)
                        .Subscribe(results => OnResultsChanged(results))
                        .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; }
        public ReactiveCommand<Unit, Unit> LoadCurrentFactoryCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoadWeighingsCommand { get; set; }

        private Factory? _factory;
        public Factory? Factory
        {
            get { return _factory; }
            set { this.RaiseAndSetIfChanged(ref _factory, value); }
        }

        [AllowNull] private ObservableCollectionExtended<Weighings> _weighings;
        public ObservableCollectionExtended<Weighings> Weighings
        {
            get { return _weighings; }
            set { this.RaiseAndSetIfChanged(ref _weighings, value); }
        }
        public async Task LoadCurrentFactoryAsync()
        {
            ApplicationState.CurrentFactory = await ApiClient.Server.Factory.GetCurrentFactoryAsync();
            Factory = ApplicationState.CurrentFactory;
        }
        public void OnLoadFailure(Exception exception)
        {
            Snackbar.Add(exception.Message);
        }
        public override async Task LoadDataByFilterAsync()
        {
            if (Factory is not null)
            {
                var message = DatabaseMessageFactory.CreateSelectWeighingsCommand(Factory, Factory.DatabaseConnection, Filter);
                await CommunicationService.Messages.Database.Weighings.SendMessageAsync(message);
            }
        }

        private void OnResultsChanged(ObservableCollectionExtended<Weighings> weighings)
        {
            Weighings.Clear();
            Weighings.AddRange(weighings);
        }
    }
}
