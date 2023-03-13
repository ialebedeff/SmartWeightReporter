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

namespace SmartWeight.Panel.Client.Pages.Factories.FactoryView
{

    public class FactoryViewModel : ViewModelBase
    {
        public FactoryViewModel(
              ApplicationState applicationState
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
            LoadFactoryCommand = ReactiveCommand.CreateFromTask<int, Factory?>(GetFactoryAsync);

            ConnectionCommand
                .Execute();
                //.Subscribe(async _ => await OnConnectedAsync());
        }

        public int FactoryId { get; set; }
        /// <summary>
        /// Get information about factory
        /// </summary>
        public ReactiveCommand<int, Factory?> LoadFactoryCommand { get; set; }
        /// <summary>
        /// Weighings by current factory
        /// </summary>
        private ObservableCollectionExtended<Weighings>? _weighings;
        /// <summary>
        /// Weighings by current factory
        /// </summary>
        public ObservableCollectionExtended<Weighings>? Weighings
        {
            get => _weighings;
            set => this.RaiseAndSetIfChanged(ref _weighings, value);
        }
        /// <summary>
        /// Information about factory
        /// </summary>
        private Factory? _factory;
        /// <summary>
        /// Information about factory
        /// </summary>
        public Factory Factory
        {
            get => _factory;
            set => this.RaiseAndSetIfChanged(ref _factory, value);
        }
        /// <summary>
        /// Resolving factory from api
        /// </summary>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        public Task<Factory?> GetFactoryAsync(int factoryId)
        { 
            return ApiClients.Server.Factory.FindAsync(factoryId);
        }
    }
}
