namespace SmartWeight.Updater.API
{
    public class ProcessApi : HttpClientBase
    {
        public ProcessApi(HttpClient httpClient) : base(httpClient)
        {
        }
        /// <summary>
        /// Завершить процесс по Идентификатору
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Task KillProccessAsync(int pid)
            => PostAsync($"process/kill?pid={pid}");
        /// <summary>
        /// Остановить сервис
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Task StopServiceAsync(string serviceName)
            => PostAsync($"process/stopService?serviceName={serviceName}");
        /// <summary>
        /// Запустить сервис
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Task RunServiceAsync(string serviceName)
            => PostAsync($"process/runService?serviceName={serviceName}");
    }
}