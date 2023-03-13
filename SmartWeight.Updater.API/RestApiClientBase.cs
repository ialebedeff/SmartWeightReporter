using System.Net.Http.Json;
using System.Text.Json;

namespace SmartWeight.Updater.API
{
    /// <summary>
    /// Базовый класс для обращения к API
    /// </summary>
    public class RestApiClientBase
    {
        private readonly JsonSerializerOptions _options;
        private readonly HttpClient _httpClient;
        public RestApiClientBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }
        /// <summary>
        /// Выполнить Get запрос
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected Task<TOutput?> GetAsync<TOutput>(string uri)
            => _httpClient.GetFromJsonAsync<TOutput>(uri);
        /// <summary>
        /// Выполнить Delete запрос
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected Task<TOutput?> DeleteAsync<TOutput>(string uri)
            => _httpClient.DeleteFromJsonAsync<TOutput>(uri);
        /// <summary>
        /// Выполнить Post запрос
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="uri"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected Task<HttpResponseMessage> PostAsync<TInput>(string uri, TInput input)
            => _httpClient.PostAsJsonAsync(uri, input);
        /// <summary>
        /// Выполнить Post запрос
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected Task PostAsync(string uri)
            => _httpClient.PostAsync(uri, null);
        /// <summary>
        /// Выполнить запрос Post
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="uri"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task<TOutput?> PostAsync<TInput, TOutput>(string uri, TInput input)
        {
            using (var response = await _httpClient.PostAsJsonAsync(uri, input))
            using (var stream = await response.Content.ReadAsStreamAsync())
                return await JsonSerializer.DeserializeAsync<TOutput>(stream, _options);
        }
        /// <summary>
        /// Выполнить запрос на Update
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="uri"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task<TOutput?> UpdateAsync<TInput, TOutput>(string uri, TInput input)
        {
            using (var response = await _httpClient.PutAsJsonAsync(uri, input))
            using (var stream = await response.Content.ReadAsStreamAsync())
                return await JsonSerializer.DeserializeAsync<TOutput>(stream, _options);
        }
        /// <summary>
        /// Скачать что-то по Url
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected async Task<byte[]?> DownloadAsync(string uri)
        {
            using (var response = await _httpClient.GetAsync(uri))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var memory = new MemoryStream())
            {
                await stream.CopyToAsync(memory);
                return memory.ToArray();
            }
        }
    }
}