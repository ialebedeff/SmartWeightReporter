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

namespace SmartWeight.Panel.Client.Dialogs
{
    public class CreateFactoryDialogViewModel : ViewModelBase, IActivatableViewModel
    {
        public CreateFactoryDialogViewModel(
            ApplicationState applicationState,
            ISnackbar snackbar,
            IDialogService dialog,
            SmartWeightApi updaterApi,
            NavigationManager navigation,
            CommunicationService<ServerConfiguration> communicationService, 
            DatabaseMessageFactory databaseMessageFactory) :
            base(applicationState, snackbar, dialog, updaterApi, navigation, communicationService, databaseMessageFactory)
        {
            AddFactoryCommand = ReactiveCommand.Create(AddFactory);
            RemoveFactoryCommand = ReactiveCommand.Create<FactoryDto>(RemoveFactory);
            CreateUserCommand = ReactiveCommand.CreateFromTask(CreateUserAsync);
            CreateUserCommand.Subscribe(_ =>
            {
                Snackbar.Add("Учётная запись успешно создана", Severity.Success);
            });
            CreateUserCommand.ThrownExceptions.Subscribe(ex =>
            {
                Snackbar.Add(ex.Message, Severity.Error);
            });   
        }
        /// <summary>
        /// Команда. Добавить производство
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddFactoryCommand { get; set; }
        /// <summary>
        /// Команда. Удалить производство
        /// </summary>
        public ReactiveCommand<FactoryDto, Unit> RemoveFactoryCommand { get; set; }
        /// <summary>
        /// Команда для создания учетной записи пользователя
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateUserCommand { get; set; }
        /// <summary>
        /// Почтовый адрес пользователя
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Логин/Юзернейм пользователя
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Производства пользователя
        /// </summary>
        public List<FactoryDto> Factories { get; set; } = new() { new FactoryDto() };
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        /// <summary>
        /// Создать учетную запись для производства
        /// </summary>
        /// <returns></returns>
        public async Task CreateUserAsync()
        {
            try
            {
                var result = await ApiClient.Server.Factory.CreateAsync(
                    Email,
                    UserName,
                    Password,
                    Factories, true);

                if (result is not null && result.Succeed is not true)
                {
                    var message = string.Join(", ", result.Errors.Select(er => er.Description));
                    throw new InvalidOperationException($"Не удалось создать учётную запись. {message}");
                }
            }
            catch { throw; }
        }
        /// <summary>
        /// Удалить завод из списка заводов пользователя
        /// </summary>
        /// <param name="factory"></param>
        private void RemoveFactory(FactoryDto factory)
        {
            Factories.Remove(factory);
            this.RaisePropertyChanged(nameof(Factories));
        }
        /// <summary>
        /// Добавить завод в список заводов пользователя
        /// </summary>
        private void AddFactory()
        {
            Factories.Add(new FactoryDto());
            this.RaisePropertyChanged(nameof(Factories));
        }
        /// <summary>
        /// Поиск производств из Jenkins
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>?> SearchAsync(string? query = "")
            => ApiClient.Server.Jenkins.SearchJobsAsync(query);
    }
}
