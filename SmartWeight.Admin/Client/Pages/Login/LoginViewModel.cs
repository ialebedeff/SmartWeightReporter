using ClientApi;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using System.Net.Http.Json;

namespace SmartWeight.Admin.Client.Pages.Login
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginRequest LoginData { get; set; } = new();
        public bool IsLoginInProcess { get; set; }
        public async Task LoginAsync()
        {
            IsLoginInProcess = true;

            try
            {
                var response = await HttpClient.PostAsJsonAsync("api/authorization/signin", LoginData);

                if (response.IsSuccessStatusCode)
                {
                    Navigation.NavigateTo("/factories");
                }
            }
            catch (ApiException ex)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;

                if ((HttpStatusCode)ex.StatusCode is HttpStatusCode.Unauthorized)
                {
                    Snackbar.Add("Не верный логин или пароль", Severity.Error);
                }
                if ((HttpStatusCode)ex.StatusCode is HttpStatusCode.InternalServerError)
                {
                    Snackbar.Add("Произошла неизвестная ошибка");
                }
            }

            IsLoginInProcess = false;
        }
    }
    public class ViewModelBase : MudComponentBase
    {
        [Inject] public SmartWeightApi ApiClient { get; set; } = null!;
        [Inject] public ISnackbar Snackbar { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;
        [Inject] public IDialogService Dialog { get; set; } = null!;
        [Inject] public HttpClient HttpClient { get; set; } = null!;
    }

    public class ApplicationState
    {
        private readonly SmartWeightApi _client;
        public ApplicationState(SmartWeightApi smartWeightApi)
        {
            _client = smartWeightApi;
        }
        public Task<User> CurrentUser => _client.GetCurrentUserAsync();
    }
}
