namespace SmartWeight.Updater.API
{
    /// <summary>
    /// Сервисное API
    /// </summary>
    public class ServiceApi
    {
        public ServiceApi(HttpClient httpClient)
        {
            Process = new ProcessApi(httpClient);
        }
        /// <summary>
        /// Процессы
        /// </summary>
        public ProcessApi Process { get; set; }
    }
}