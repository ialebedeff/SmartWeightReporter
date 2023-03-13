namespace SmartWeight.Updater.API
{
    /// <summary>
    /// 
    /// </summary>
    public class RestApiClients
    {
        public RestApiClients(HttpClient serverClient, HttpClient serviceClient)
        {
            Server = new ServerApi(serverClient);
            Service = new ServiceApi(serviceClient);
        }
        /// <summary>
        /// Запросы к сервису на стороне клиента
        /// </summary>
        public ServiceApi Service { get; set; }
        /// <summary>
        /// Запросы к главному серверу
        /// </summary>
        public ServerApi Server { get; set; }
    }
}