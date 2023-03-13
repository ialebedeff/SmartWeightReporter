using ClientApi;
using SmartWeight.Admin.Client.Pages.Login;
using System.Net;
using System.Net.Http.Json;

namespace SmartWeight.Admin.Client.Dialogs
{
    public class CreateFactoryDialogViewModel : ViewModelBase
    {
        public CreateUserRequest User { get; set; } = new();
        public async Task CreateUserAsync()
        {
            try
            {

                var response = await HttpClient.PostAsJsonAsync("api/User/CreateUser", User);

                if (response.IsSuccessStatusCode)
                {
                    Snackbar.Add("Пользователь успешно создан", MudBlazor.Severity.Success);
                }

                //if (!result.Succeeded)
                //{
                //    foreach (var error in result.Errors)
                //    {
                //        Snackbar.Add(string.Format(
                //            "Ответ от сервера: {0}, текст ошибки: {1}",
                //            error.Code, error.Description));
                //    }
                //}
                //else { Snackbar.Add("Пользователь успешно создан", MudBlazor.Severity.Success); }
            }
            catch (ApiException ex)
            {
                if ((HttpStatusCode)ex.StatusCode is HttpStatusCode.InternalServerError)
                {
                    Snackbar.Add($"Произошла ошибка на сервере: {ex.Message}", MudBlazor.Severity.Error);
                }
                
            }
        }
    }
}
