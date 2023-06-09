﻿using Communication;
using Communication.Configurator;
using Communication.Server;
using DynamicData.Binding;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Data;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SmartWeight.Panel.Client.Pages.Factories.FactorySelection
{
    public class FactorySelectionViewModel : ViewModelBase, IActivatableViewModel
    {
        public FactorySelectionViewModel(ApplicationState applicationState, ISnackbar snackbar, IDialogService dialog, RestApiClients updaterApi, NavigationManager navigation, CommunicationService<ServerConfiguration> communicationService, DatabaseMessageFactory databaseMessageFactory) : base(applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
        {
            SignOutCommand = ReactiveCommand.CreateFromTask(_ => SignOutAsync());
            NavigateToLoginPageCommand = ReactiveCommand.Create(() => Navigation.NavigateTo("/login"));
            LoadCurrentUserFactoriesCommand = ReactiveCommand.CreateFromTask(_ => LoadCurrentUserFactoriesAsync());
            LoadConnectedClientsCommand = ReactiveCommand.CreateFromTask(_ => RequestToConnectedClientsAsync());
            SelectFactoryCommand = ReactiveCommand.Create(ApproveSelectedFactory);
            NavigateToChartsCommand = ReactiveCommand.Create(() => Navigation.NavigateTo("/charts"));
            ApproveSelectionAndNavigateCommand = ReactiveCommand.CreateCombined(
                new ReactiveCommand<Unit,Unit>[] 
                { 
                    SelectFactoryCommand, 
                    NavigateToChartsCommand 
                });
            SignOutAndNavigateToLoginCommand = ReactiveCommand.CreateCombined(
                new ReactiveCommand<Unit, Unit>[]
                {
                    SignOutCommand,
                    NavigateToLoginPageCommand
                });
            

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.SelectedFactories)
                    .Subscribe(result => this.RaisePropertyChanged(nameof(IsSelectionDisabled)))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.CommunicationService.Messages.UserConnectionStateHub.ConnectionStates.Results)
                    .Subscribe(result => 
                    {
                        var connectionState = result.FirstOrDefault();

                        if (connectionState != null) 
                        if (connectionState.State is ConnectionState.Open)
                            ApplicationState.AddClient(connectionState.HubClient);
                        else
                            ApplicationState.RemoveClient(connectionState.HubClient);
                    })
                    .DisposeWith(disposables);
                // Загрузка текущего пользователя
                LoadCurrentUserFactoriesCommand.ThrownExceptions
                    .Subscribe(exception => this.Snackbar.Add(exception.Message, Severity.Error))
                    .DisposeWith(disposables);

                // Подписка на коллекцию клиентов
                this.WhenAnyValue(x => x.CommunicationService.Messages.ClientsHub.Clients.Results)
                    .Subscribe(result => ApplicationState.ConnectedClients = result)
                    .DisposeWith(disposables);

                // Команда на подключение к серверу по SignalR
                ConnectionCommand
                    .Execute()
                    .Subscribe(result =>
                    {
                        // Загрузка подключенных производств
                        // текущего пользователя
                        LoadConnectedClientsCommand
                       .Execute()
                       .Subscribe(result =>
                       {
                           // Загрузка всех производств
                           // пользователя
                           LoadCurrentUserFactoriesCommand
                               .Execute()
                               .Subscribe(factories =>
                               {
                                   UserFactories.Clear();
                                   UserFactories.AddRange(factories);
                                   this.RaisePropertyChanged(nameof(UserFactories));
                               })
                               .DisposeWith(disposables);
                       });
                    }).DisposeWith(disposables);

               
                // Обработка ошибок при
                // выполнении команд
                LoadConnectedClientsCommand.ThrownExceptions
                    .Subscribe(exception => this.Snackbar.Add(exception.Message, Severity.Error))
                    .DisposeWith(disposables);
                ApproveSelectionAndNavigateCommand.ThrownExceptions
                    .Subscribe(exception => this.Snackbar.Add(exception.Message, Severity.Error))
                    .DisposeWith(disposables);
                SignOutAndNavigateToLoginCommand.ThrownExceptions
                    .Subscribe(exception => this.Snackbar.Add(exception.Message, Severity.Error))
                    .DisposeWith(disposables);
            });
        }
        /// <summary>
        /// Команда для редиректа на окно авторизации
        /// </summary>
        public ReactiveCommand<Unit, Unit> NavigateToLoginPageCommand { get; set; }
        /// <summary>
        /// Команда для выход из аккаунта
        /// </summary>
        public ReactiveCommand<Unit, Unit> SignOutCommand { get; set; }
        /// <summary>
        /// Выйти из аккаунта и перейти на страницу авторизации
        /// </summary>
        public CombinedReactiveCommand<Unit, Unit> SignOutAndNavigateToLoginCommand { get; set; }
        /// <summary>
        /// Подтвердить выбор производства
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectFactoryCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCommand<Unit, Unit> NavigateToChartsCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CombinedReactiveCommand<Unit, Unit> ApproveSelectionAndNavigateCommand { get; set; }
        /// <summary>
        /// Выполнить подключение к серверу по SignalR
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartConnectionCommand { get; set; }
        /// <summary>
        /// Получить заводы пользователя
        /// </summary>
        public ReactiveCommand<Unit, IEnumerable<Factory>?> LoadCurrentUserFactoriesCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadConnectedClientsCommand { get; set; }
        /// <summary>
        /// Заводы пользователя
        /// </summary>
        private ObservableCollectionExtended<Factory> _userFactories = new();
        /// <summary>
        /// Заводы пользователя
        /// </summary>
        public ObservableCollectionExtended<Factory> UserFactories
        {
            get { return _userFactories; }
            set { this.RaiseAndSetIfChanged(ref _userFactories, value); }
        }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        public bool IsSelectionDisabled => SelectedFactories is null || 
                                           SelectedFactories.Count() is 0;
        /// <summary>
        /// Получить заводы пользователя по API
        /// </summary>
        /// <returns></returns>
        private Task<IEnumerable<Factory>?> LoadCurrentUserFactoriesAsync()
            => ApiClients.Server.Factory.GetCurrentUserFactoriesAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task RequestToConnectedClientsAsync()
            => CommunicationService.Messages.ClientsHub.Clients.SendMessageAsync();
        /// <summary>
        /// Выбранные производства / производство
        /// </summary>
        public IEnumerable<Factory> SelectedFactories { get; set; }
        /// <summary>
        /// Подтвердить выбор производства
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void ApproveSelectedFactory()
        {
            if (SelectedFactories is null || SelectedFactories.Count() == 0) 
                throw new ArgumentNullException(nameof(SelectedFactories));

            ApplicationState.CurrentFactory = SelectedFactories.First();
        }
        /// <summary>
        /// Выполнить выход из аккаунт
        /// </summary>
        /// <returns></returns>
        private Task SignOutAsync()
            => ApiClients.Server.Authorization.SignOutAsync();
    }
}
