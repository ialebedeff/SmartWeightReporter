using Communication;
using Communication.Configurator;
using Communication.Server;
using DynamicData.Binding;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Admin.Client.Pages.Factories;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;

namespace SmartWeight.Panel.Client.Pages.Users.Collection
{
    public class UsersManagementViewModel : ViewModelBase
    {
        public UsersManagementViewModel(SearchViewModel<User> searchViewModel, ApplicationState applicationState, ISnackbar snackbar, IDialogService dialog, RestApiClients updaterApi, NavigationManager navigation, CommunicationService<ServerConfiguration> communicationService, DatabaseMessageFactory databaseMessageFactory) : base(applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
        {
            Search = searchViewModel;
            Search.SearchFunction = query => updaterApi.Server.User.SearchAsync(query);
            Search.Search.Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(Search.SearchResults));
            });
            Search.Search.Execute();
        }

        /// <summary>
        /// Поиск по пользователям
        /// </summary>
        public SearchViewModel<User> Search { get; set; }
        /// <summary>
        /// Пользователи
        /// </summary>
        public ObservableCollectionExtended<User> Users { get; set; } = new();
    }
}
