using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.Security.Claims;

namespace SmartWeight.Updater.API
{
    /// <summary>
    /// Api для авторизации на сервере
    /// </summary>
    public class AuthorizationApi : RestApiClientBase
    {
        public AuthorizationApi(HttpClient httpClient) : base(httpClient) { }
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task SignInAsync(string login, string password)
        {
            try
            {
                var request = new LoginRequest(login, password);
                var response = await PostAsync<LoginRequest, OperationResult>("api/authorization/signin", request);

                ArgumentNullException.ThrowIfNull(response);
                ApiException.ThrowIfFalse(response.Succeed, "Не верный логин или пароль");

                AuthorizationStateChanged?.Invoke(this, new AuthorizationState() { IsAuthorized = true });
            }
            catch (Exception) { throw; }
        }
        /// <summary>
        /// Получить AuthenticationState пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await GetAsync<AuthenticationData>("api/Authorization/GetAuthenticationState");
                
                ArgumentNullException.ThrowIfNull(result);

                var identity = new ClaimsIdentity(result.User.Claims
                    .Select(c => new Claim(
                          type: c.Type
                        , value: c.Value
                        , valueType: c.ValueType
                        , issuer: c.Issuer
                        , originalIssuer: c.OriginalIssuer))
                        , authenticationType: result.User.AuthenticationType);

                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);

            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Выход из аккаунта
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            try
            {
                await PostAsync("api/authorization/signout");
                AuthorizationStateChanged?.Invoke(this, new AuthorizationState() { IsAuthorized = false });
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Ивент на состояние авторизации пользователя
        /// </summary>
        public event EventHandler<AuthorizationState>? AuthorizationStateChanged;
    }
}