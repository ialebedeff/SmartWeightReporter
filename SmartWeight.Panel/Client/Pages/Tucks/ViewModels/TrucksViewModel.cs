using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities.Database;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Admin.Client.Pages.Factories;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Panel.Client.Pages.Tucks.Views;
using SmartWeight.Updater.API;
using System.Reactive;
using System.Reactive.Disposables;

namespace SmartWeight.Panel.Client.Pages.Tucks.ViewModels
{
    public class TrucksViewModel : ViewModelBase, IActivatableViewModel
    {
        public TrucksViewModel(ApplicationState applicationState
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
            LoadTrucksCommand = ReactiveCommand.CreateFromTask(LoadTrucksAsync);
            SearchTrucksCommand = ReactiveCommand.CreateFromTask<string>(SearchTrucksAsync);

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.TransportNumber)
                    .Subscribe(query => SearchTrucksCommand.Execute(query))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.CommunicationService.Messages.DatabaseHub.Cars.Results)
                     .Subscribe(result => Cars = result)
                     .DisposeWith(disposables);

                this.LoadTrucksCommand.ThrownExceptions
                     .Subscribe(exception => Snackbar.Add($"Не удалось загрузить автомобили. Ошибка: {exception.Message}", Severity.Error))
                     .DisposeWith(disposables);

            });

            LoadTrucksCommand.Execute();
        }
        /// <summary>
        /// Машины из базы клиенты
        /// </summary>
        private IEnumerable<Truck>? _cars;
        /// <summary>
        /// Машины из базы клиенты
        /// </summary>
        public IEnumerable<Truck>? Cars
        {
            get { return this._cars; }
            set { this.RaiseAndSetIfChanged(ref _cars, value); }
        }
        /// <summary>
        /// Номер автомобиля для поиск
        /// </summary>
        private string _transportNumber;
        /// <summary>
        /// Номер автомобиля для поиск
        /// </summary>
        public string TransportNumber
        {
            get { return this._transportNumber; }
            set { this.RaiseAndSetIfChanged(ref _transportNumber, value); }
        }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        /// <summary>
        /// Команда для загрузки рабочих автомобилей
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadTrucksCommand { get; set; }
        /// <summary>
        /// Команда для запроса траков по гос номеру
        /// </summary>
        public ReactiveCommand<string, Unit> SearchTrucksCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenTruckDetailsCommand { get; set; }
        /// <summary>
        /// Отправить запрос на загрузку рабочих автомобилей
        /// </summary>
        /// <returns></returns>
        private async Task LoadTrucksAsync()
        {
            if (ApplicationState.CurrentFactory is not null)
            {
                var message = DatabaseMessageFactory.CreateSelectWorkCarsCommand(ApplicationState.CurrentFactory);
                await CommunicationService.Messages.DatabaseHub.Cars.SendMessageAsync(message);
            }
        }
        /// <summary>
        /// Функция поиска трака по гос. номеру
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private async Task SearchTrucksAsync(string query)
        {
            if (ApplicationState.CurrentFactory is not null)
            {
                var message = DatabaseMessageFactory.CreateSearchWorkCarsCommand(query, ApplicationState.CurrentFactory);
                await CommunicationService.Messages.DatabaseHub.Cars.SendMessageAsync(message);
            }
        }
    }
}
