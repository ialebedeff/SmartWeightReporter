namespace SmartWeight.Updater.API
{
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