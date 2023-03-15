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

namespace SmartWeight.Panel.Client.Pages.Factories.FactorySelection
{
    public class FactoryCardViewModel : ViewModelBase, IActivatableViewModel
    {
        public FactoryCardViewModel(ApplicationState applicationState, ISnackbar snackbar, IDialogService dialog, RestApiClients updaterApi, NavigationManager navigation, CommunicationService<ServerConfiguration> communicationService, DatabaseMessageFactory databaseMessageFactory) : base(applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
        {
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.CommunicationService.Messages.UserConnectionStateHub.ConnectionStates.Results)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(IsOnline)))
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ApplicationState.ConnectedClients)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(IsOnline)))
                    .DisposeWith(disposables);
            });   
        }
        /// <summary>
        /// Текущее производство
        /// </summary>
        public Factory CurrentFactory { get; set; } = null!;
        /// <summary>
        /// Команда для выбора производства
        /// и редирект на страницу графиков
        /// </summary>
        public ReactiveCommand<Factory, Unit> SelectFactoryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        // public ReactiveCommand<Factory, Unit> IsFactoryOnlineCommand { get; set; }
        /// <summary>
        /// Если подключение не активно, то 
        /// нельзя выбрать в качестве обсервера
        /// </summary>
        public bool IsSelectionDisabled => IsOnline is not true;
        /// <summary>
        /// Состояние подключения
        /// </summary>
        public bool IsOnline => ApplicationState.ConnectedClients
            .Any(client => client.Factory?.Id == CurrentFactory.Id);
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
    }
}
