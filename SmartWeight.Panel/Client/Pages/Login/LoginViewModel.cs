using Communication;
using Communication.Configurator;
using Communication.Server;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Updater.API;
using System.Reactive;
using System.Threading.Tasks;

namespace SmartWeight.Panel.Client.Pages.Login
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthenticationStateProvider _authenticationState;
        public LoginViewModel(
              ApplicationState applicationState
            , ISnackbar snackbar
            , IDialogService dialog
            , RestApiClients updaterApi
            , NavigationManager navigation
            , AuthenticationStateProvider authenticationStateProvider
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
            _authenticationState = authenticationStateProvider;

            CheckAuthenticationStateCommand = ReactiveCommand.CreateFromTask<Unit, AuthenticationState>(_ => GetAuthenticationStateAsync());
            CheckAuthenticationStateCommand
                .Execute()
                .Subscribe(async state =>
                {
                    if (state.User.Identity is not null)
                    if (state.User.Identity.IsAuthenticated)
                    {
                        await RedirectAsync(state);
                    }
                });

            LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
            LoginCommand.ThrownExceptions.Subscribe(OnFailure);
            LoginCommand.Subscribe(_ => RedirectAsync());
        }
        public ReactiveCommand<Unit, AuthenticationState> CheckAuthenticationStateCommand { get; set; }
        /// <summary>
        /// Обработка успешной авторизации
        /// </summary>
        public IObserver<Unit> OnLoginSuccessObserver { get; set; }
        /// <summary>
        /// Команда для авторизации
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }
        /// <summary>
        /// Авторизоваться
        /// </summary>
        /// <returns></returns>
        public Task LoginAsync() => ApiClients.Server.Authorization
                   .SignInAsync(Login, Password);
        /// <summary>
        /// Логин / Юзернейм
        /// </summary>
        private string? _login;
        /// <summary>
        /// Логин / Юзернейм
        /// </summary>
        public string? Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }
        /// <summary>
        /// Статус авторизации для пользователя
        /// </summary>
        private string? _status;
        /// <summary>
        /// Статус авторизации для пользователя
        /// </summary>
        public string? Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
        /// <summary>
        /// Пароль
        /// </summary>
        private string? _password;
        /// <summary>
        /// Пароль
        /// </summary>
        public string? Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }
        /// <summary>
        /// Обработка ошибок при авторизации
        /// </summary>
        /// <param name="exception"></param>
        public void OnFailure(Exception exception)
        {
            Status = exception.Message;
        }
        /// <summary>
        /// Перенаправление пользователя 
        /// в зависимости от его роли
        /// </summary>
        /// <returns></returns>
        public async Task RedirectAsync(AuthenticationState? state = null)
        {
            if (state is null)
            {
                await _authenticationState
                    .GetAuthenticationStateAsync()
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            ApplicationState.AuthenticationState = task.Result;
                        }
                        if (task.Result.User.IsInRole("Admin"))
                        {
                            Navigation.NavigateTo("/factories");
                        }
                        else { Navigation.NavigateTo("/select/factories"); }
                    });
            }
            else
            {
                if (state.User.IsInRole("Admin"))
                {
                    Navigation.NavigateTo("/factories");
                }
                else 
                { Navigation.NavigateTo("/select/factories"); }
            }
        }

        private Task<AuthenticationState> GetAuthenticationStateAsync()
            => ApiClients.Server.Authorization.GetAuthenticationStateAsync();
    }
}
