namespace SmartWeight.Updater.API
{
    public class JenkinsApi : RestApiClientBase
    {
        public JenkinsApi(HttpClient httpClient) : base(httpClient) { }
        /// <summary>
        /// Поиск работ в Jenkins. Используется как получение списка всех заводов
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>?> SearchJobsAsync(string? query = "")
            => GetAsync<IEnumerable<string>>($"api/jenkins/SearchJobs?query={query}");
    }
}