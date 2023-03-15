using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities.Database;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Dialogs;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Reactive;

namespace SmartWeight.Panel.Client.Pages.Tucks.ViewModels
{
    public class TruckViewModel : ViewModelBase, IActivatableViewModel
    {
        public TruckViewModel(ApplicationState applicationState
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
            OpenTruckDetailsCommand = ReactiveCommand.CreateFromTask<Truck, IDialogReference>(OpenTruckDetailsDialogAsync);
        }

        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        /// <summary>
        /// Команда для открытия диалогового окна с траком
        /// </summary>
        public ReactiveCommand<Truck, IDialogReference> OpenTruckDetailsCommand { get; set; }
        /// <summary>
        /// Открыть диалоговое окно с детальной информацией по траку
        /// </summary>
        /// <param name="truck"></param>
        /// <returns></returns>
        private Task<IDialogReference> OpenTruckDetailsDialogAsync(Truck truck)
        {
            var dialogOptions = new DialogOptions()
            {
                CloseButton = true,
                CloseOnEscapeKey = true,
                DisableBackdropClick = true,
                FullScreen = false,
                MaxWidth = MaxWidth.ExtraLarge
            };

            var dialogParameters = new DialogParameters()
            {
                ["Truck"] = truck
            };

            return Dialog.ShowAsync<TruckDetailsDialog>(
                ""
                , dialogParameters
                , dialogOptions);
        }
    }
}
