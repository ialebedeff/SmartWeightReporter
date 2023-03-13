using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client;
using SmartWeight.Panel.Client.Dialogs;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Reactive;
using System.Reactive.Linq;

namespace SmartWeight.Admin.Client.Pages.Factories
{
    public class FactoryCollectionViewModel : ViewModelBase
    {
        public FactoryCollectionViewModel(
              ApplicationState applicationState
            , ISnackbar snackbar
            , IDialogService dialog
            , RestApiClients updaterApi
            , NavigationManager navigation
            , SearchViewModel<Factory> searchViewModel
            , CommunicationService<ServerConfiguration> communicationService
            , DatabaseMessageFactory databaseMessageFactory) 
            : base(
                    applicationState
                  , snackbar
                  , dialog
                  , updaterApi
                  , navigation
                  , communicationService
                  , databaseMessageFactory)
        {
            Search = searchViewModel;
            Search.SearchFunction = query => updaterApi.Server.Factory.Search(query);
            // Creating the commands 
            // Creating a command that opens
            // a dialog for creating an account
            CreateFactoryCommand = ReactiveCommand.CreateFromTask(OpenCreateFactoryDialogAsync);
            // Subscribing to the dialog window closing event
            // We subscribe to the closing event of the dialog box
            // in order to update the latest data on all new factories
            CreateFactoryCommand.Subscribe(async _ => await OnDialogClosed());
            // Creating a team that opens a page
            // with information about the selected plant
            OpenFactoryCommand = ReactiveCommand.Create<Factory>(OpenFactoryPage);
            // We subscribe to the command execution event in
            // order to update the collection with found factories
            //Search.Search.Subscribe(x =>
            //{
            //    this.RaisePropertyChanged(nameof(Search.SearchResults));
            //});
            // Execute the search command
            this.WhenAnyValue(x => x.Search)
                .Subscribe(x => this.RaisePropertyChanged(nameof(x.SearchResults)));

            Search.Search.Execute();
        }
        /// <summary>
        /// Search ViewModel
        /// </summary>
        public SearchViewModel<Factory> Search { get; set; }
        /// <summary>
        /// Opens a dialog to create a new factory account
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateFactoryCommand { get; set; }
        /// <summary>
        /// Open a page with factory information
        /// </summary>
        public ReactiveCommand<Factory, Unit> OpenFactoryCommand { get; set; }
        /// <summary>
        /// Factories matching the search string 
        /// </summary>
        // public ObservableCollection<Factory> Factories { get; set; } = new();
        /// <summary>
        /// Opens a dialog to create a new user account
        /// </summary>
        public async Task OpenCreateFactoryDialogAsync()
        {
            // Open dialog
            var reference = await Dialog.ShowAsync<CreateFactoryDialog>(string.Empty, new DialogOptions()
            {
                MaxWidth = MaxWidth.Medium
            });

            // Waiting for exit from dialog
            var result = await reference.Result;
        }
        /// <summary>
        /// Reload search when dialog has been closed
        /// </summary>
        /// <returns></returns>
        public async Task OnDialogClosed()
            => await Search.Search.Execute();
        /// <summary>
        /// Open a page with factory information
        /// </summary>
        public void OpenFactoryPage(Factory factory)
            => Navigation.NavigateTo($"/factory/{factory.Id}", false);
    }
}
