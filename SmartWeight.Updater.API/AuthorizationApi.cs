using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.Security.Claims;

namespace SmartWeight.Updater.API
{
    public class AuthorizationApi : HttpClientBase
    {
        public AuthorizationApi(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task SignInAsync(string login, string password)
        {
            try
            {
                var response = await PostAsync<LoginRequest, OperationResult>("api/authorization/signin", new LoginRequest()
                {
                    Login = login,
                    Password = password
                });

                if (response?.Succeed == true)
                {
                    AuthorizationStateChanged?.Invoke(this, new AuthorizationState() { IsAuthorized = true });
                }
                else
                {
                    throw new ApiException("Не верный логин или пароль.");
                }
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> IsAuthorizedAsync()
        {
            try
            {
                return GetAsync<bool>("api/authorization/AuthorizationState");
            }
            catch { throw; }
        }

        public async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await GetAsync<AuthenticationData>("api/Authorization/GetAuthenticationState");

                if (result is not null)
                {
                    var identity = new ClaimsIdentity(result.User.Claims
                        .Select(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer, c.OriginalIssuer)),
                        result.User.AuthenticationType);
                    var principal = new ClaimsPrincipal(identity);

                    return new AuthenticationState(principal);
                }
            }
            catch
            {

            }

            return new AuthenticationState(new ClaimsPrincipal());
        }

        public async Task SignOutAsync()
        {
            try
            {
                await PostAsync("api/authorization/signout");
                AuthorizationStateChanged?.Invoke(this, new AuthorizationState() { IsAuthorized = false });
            }
            catch { }
        }

        public event EventHandler<AuthorizationState>? AuthorizationStateChanged;
    }
}