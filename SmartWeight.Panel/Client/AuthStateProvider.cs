using Microsoft.AspNetCore.Components.Authorization;
using SmartWeight.Updater.API;

namespace SmartWeight.Admin.Client.Pages.Factories
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly SmartWeightApi _updaterApi;
        public AuthStateProvider(
            SmartWeightApi updaterApi)
        {
            _updaterApi = updaterApi;
        }
        /// <summary>
        /// Получить AuthenticationState пользователя
        /// </summary>
        /// <returns></returns>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => _updaterApi.Server.Authorization.GetAuthenticationStateAsync();
    }
}
