using Entities;

namespace SmartWeight.Updater.API
{
    /// <summary>
    /// Апи для работы с пользователями
    /// </summary>
    public class UserApi : RestApiClientBase
    {
        public UserApi(HttpClient httpClient) : base(httpClient) { }
        /// <summary>
        /// Поиск пользователя по 
        /// Email, Username, Phone Number
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<IEnumerable<User>?> SearchAsync(string? query = null)
            => query is null ? 
                GetAsync<IEnumerable<User>>($"api/user/GetAllUsers") : 
                GetAsync<IEnumerable<User>>($"api/user/Search?query={query}");
    }
}