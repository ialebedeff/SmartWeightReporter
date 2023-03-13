using Entities;

namespace SmartWeight.Updater.API
{
    public class FactoryApi : RestApiClientBase
    {
        public FactoryApi(HttpClient httpClient) : base(httpClient) { }
        /// <summary>
        /// Создать учётную запись для завода
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="companyTitle"></param>
        /// <param name="isUpdatesEnabled"></param>
        /// <returns></returns>
        public Task<OperationResult?> CreateAsync(
            string email,
            string userName,
            string password,
            IEnumerable<FactoryDto> factories,
            bool isUpdatesEnabled = true)
            => PostAsync<CreateFactoryUserRequest, OperationResult>("api/user/CreateUser", new CreateFactoryUserRequest()
            {
                  Email = email
                , IsUpdateEnabled = isUpdatesEnabled
                , Password = password
                , Factories = factories
                , UserName = userName
            });
        /// <summary>
        /// Получить производство, под которым выполнен вход
        /// </summary>
        /// <returns></returns>
        public Task<Factory?> GetCurrentFactoryAsync()
            => GetAsync<Factory>("api/factory/GetCurrentFactory");
        /// <summary>
        /// Получить производства пользователя под которым выполнен вход
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Factory>?> GetCurrentUserFactoriesAsync()
            => GetAsync<IEnumerable<Factory>>("api/factory/GetCurrentUserFactories");
        /// <summary>
        /// Получить все производства
        /// </summary>
        /// <returns></returns>
        public Task<List<Factory>?> GetAllAsync()
            => GetAsync<List<Factory>>("api/factory/GetAllFactories");
        /// <summary>
        /// Обновить данные о заводе
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public Task<OperationResult<Factory>?> UpdateAsync(Factory factory)
            => UpdateAsync<Factory, OperationResult<Factory>>("api/factory/UpdateFactory", factory);
        /// <summary>
        /// Удалить завод
        /// </summary>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        public Task<OperationResult?> DeleteAsync(int factoryId)
            => DeleteAsync<OperationResult>($"api/factory/DeleteFactory?factoryId={factoryId}");
        /// <summary>
        /// Получить завод по идентификатору пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<Factory?> GetByUserId(int userId)
            => GetAsync<Factory>("api/factory/GetFactoryByUser");
        /// <summary>
        /// Поиск завода по Id
        /// </summary>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        public Task<Factory?> FindAsync(int factoryId)
            => GetAsync<Factory>($"api/factory/GetFactory?factoryId={factoryId}");
        /// <summary>
        /// Поиск завода по email, username или названию
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<IEnumerable<Factory>?> Search(string? query = null)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                return GetAsync<IEnumerable<Factory>>($"api/factory/search?query={query}");
            }

            return GetAsync<IEnumerable<Factory>>($"api/factory/search");
        }
    }
}