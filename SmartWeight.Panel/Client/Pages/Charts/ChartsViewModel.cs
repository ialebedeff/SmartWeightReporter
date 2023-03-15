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
using System.Reactive.Linq;

namespace SmartWeight.Panel.Client.Pages.Charts
{
    public class ChartsViewModel : FilterableViewModel, IActivatableViewModel
    {
        public ChartsViewModel(
            ChartJS chartJS,
            Filter filter,
            ApplicationState applicationState,
            ISnackbar snackbar,
            IDialogService dialog,
            RestApiClients updaterApi,
            NavigationManager navigation,
            CommunicationService<ServerConfiguration> communicationService,
            DatabaseMessageFactory databaseMessageFactory) : base(filter, applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
        {
            ChartJS = chartJS;
            Activator = new ViewModelActivator();

            LoadWeighingsCommand = ReactiveCommand.CreateFromTask(LoadDataByFilterAsync);
            // LoadCurrentFactoryCommand = ReactiveCommand.CreateFromTask(LoadCurrentFactoryAsync);

            /*
                Т.к используются подписки необходимо от них избавляться
                при деактивации вью модели, иначе, будет хуёво
             */
            this.WhenActivated(blocks =>
            {
                var exceptionObservable = this
                    .LoadWeighingsCommand.ThrownExceptions
                    .Subscribe(OnLoadFailure)
                    .DisposeWith(blocks);
                var weighingsResultObservable = this
                    .WhenAnyValue(x => x.CommunicationService.Messages.DatabaseHub.Weighings.Results)
                    .Subscribe(result => ResultChanged(result))
                    .DisposeWith(blocks);
            });

            LoadWeighingsCommand.Execute();
        }
        /// <summary>
        /// Управление графиками
        /// </summary>
        public ChartJS ChartJS { get; set; }
        /// <summary>
        /// Команда для загрузки информации по текущему заводу
        /// </summary>
        /// public ReactiveCommand<Unit, Unit> LoadCurrentFactoryCommand { get; set; }
        /// <summary>
        /// Команда для загрузки списка взвешиваний
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadWeighingsCommand { get; set; }
        /// <summary>
        /// Активатор ViewModel-ей
        /// </summary>
        public ViewModelActivator Activator { get; set; }

        private int _weighingsCount;
        /// <summary>
        /// Количество взвешиваний по списку
        /// </summary>
        public int WeighingsCount
        {
            get { return _weighingsCount; }
            set { this.RaiseAndSetIfChanged(ref _weighingsCount, value); }
        }

        private double _totalWeight;
        /// <summary>
        /// Сумма масс всех взвешиваний
        /// </summary>
        public double TotalWeight
        {
            get { return _totalWeight; }
            set { this.RaiseAndSetIfChanged(ref _totalWeight, value); }
        }

        private int _totalWeighingsCount;
        /// <summary>
        /// Количество взвешваний
        /// </summary>
        public int TotalWeighingsCount
        {
            get { return _totalWeighingsCount; }
            set { this.RaiseAndSetIfChanged(ref _totalWeighingsCount, value); }
        }
        /// <summary>
        /// Загрузить информацию по текущему заводу
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Вывести ошибку при неудачной обработке загрузке
        /// </summary>
        /// <param name="exception"></param>
        public void OnLoadFailure(Exception exception)
        {
            Snackbar.Add(exception.Message);
        }
        /// <summary>
        /// Коллекция взвешиваний была изменена
        /// </summary>
        /// <param name="weighings"></param>
        public async void ResultChanged(ObservableCollectionExtended<Weighings> weighings)
        {
            // Weighings = weighings;

            // Получаем количество тон по дням
            // Пример: weights[0] = 20 - это значит что в первый день было 20 тонн, 
            //         weights[1] = 30 - а во второй 30 тонн
            var weights = weighings
                .OrderBy(w => w.DatetimeFirst)
                .GroupBy(value => value.DatetimeFirst.ToString("yyyy.MM.dd"))
                .Select(value => value.Sum(item => (item.StableWeight ?? 0 + item.CorrectWeight)) / 1000);

            // Получаем даты
            var dates = weighings
                .Select(w => w.DatetimeFirst.ToString("dd.MM.yyyy"))
                .Distinct();

            // Получаем количество взвешиваний по суткам
            var times = weighings
                .OrderBy(w => w.DatetimeFirst)
                .GroupBy(value => value.DatetimeFirst.ToString("dd.MM.yyyy"))
                .Select(value => value.Count());

            // Получаем сумму массы всех взвешиваний
            TotalWeight = ((double)weighings
                .OrderBy(w => w.DatetimeFirst)
                .GroupBy(value => value.DatetimeFirst.ToString("yyyy.MM.dd"))
                .Select(value => value.Sum(item => (item.StableWeight ?? 0 + item.CorrectWeight)))
                .Sum() / 1000);

            // Получаем количество всех взвешиваний
            // за выбранный по фильтру промежуток
            WeighingsCount = times.Sum();
            TotalWeighingsCount = weighings.Count();
            // Строим график
            await ChartJS.PerformanceOnBatchChart(weights, times, dates, $"Общая масса: {TotalWeight}", $"Общее кол-во: {WeighingsCount}");
        }

        /// <summary>
        /// Загрузить данные по фильтру
        /// </summary>
        /// <returns></returns>
        public override async Task LoadDataByFilterAsync()
        {
            if (ApplicationState.CurrentFactory is not null)
            {
                var message = DatabaseMessageFactory.CreateSelectWeighingsCommand(
                    ApplicationState.CurrentFactory, 
                    ApplicationState.CurrentFactory.DatabaseConnection, Filter);

                await CommunicationService.Messages.DatabaseHub.Weighings.SendMessageAsync(message);
            }
        }
    }
}
