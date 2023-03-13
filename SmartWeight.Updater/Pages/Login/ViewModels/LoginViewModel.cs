using ReactiveUI;
using SmartWeight.MemoryBase;
using SmartWeight.Updater.API;
using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;

namespace SmartWeight.Updater.Pages.Login.ViewModels;
public class LoginViewModel : ReactiveObject, IRoutableViewModel
{
    private string _login = string.Empty;
    private string _password = string.Empty;
    private readonly SmartWeightApi _clientApi;
    private readonly AuthorizationStoreProvider _authorizationStore;

    public LoginViewModel(
        SmartWeightApi updaterApi, 
        AuthorizationStoreProvider authorizationStore)
    {
        _clientApi = updaterApi;
        _authorizationStore = authorizationStore;

        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
        LoginCommand.ThrownExceptions.Subscribe(e => OnLoginError(e));
        LoginCommand.Subscribe(_ => OnLoginSuccess());

        SetAuthorizationData();
    }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }
    public void SetAuthorizationData()
    {
        var authorizationStore = _authorizationStore.GetLastSuccessAuthorization();

        if (authorizationStore != null)
        {
            Login = authorizationStore.Login;
            Password = authorizationStore.Password;
        }
    }
    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string? UrlPathSegment { get; set; }
    public IScreen HostScreen { get; set; }
    public Task LoginAsync() => _clientApi.Server.Authorization.SignInAsync(Login, Password);
    public void OnLoginError(Exception exception)
    {
        if (exception is ApiException api)
        { 
            MessageBox.Show(api.Message);
        }
    }

    public void OnLoginSuccess() => _authorizationStore.AddAuthorization(Login, Password, true);
}
