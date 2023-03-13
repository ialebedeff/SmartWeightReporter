using System.Net;

namespace SmartWeight.Updater.API
{
    public class ServerApi
    {
        public ServerApi(HttpClient httpClient)
        {
            Authorization = new AuthorizationApi(httpClient);
            Factory = new FactoryApi(httpClient);
            Update = new UpdatesApi(httpClient);
            Note = new NoteApi(httpClient);
            Jenkins = new JenkinsApi(httpClient);
            User = new UserApi(httpClient);
        } 
        /// <summary>
        /// Авторизация
        /// </summary>
        public AuthorizationApi Authorization { get; set; }
        /// <summary>
        /// Производства
        /// </summary>
        public FactoryApi Factory { get; set; }
        /// <summary>
        /// Обновления
        /// </summary>
        public UpdatesApi Update { get; set; }
        /// <summary>
        /// Заметки
        /// </summary>
        public NoteApi Note { get; set; }
        /// <summary>
        /// Jenkins
        /// </summary>
        public JenkinsApi Jenkins { get; set; }
        /// <summary>
        /// Api для взаимодействия с пользователями
        /// </summary>
        public UserApi User { get; set; }
    }
}