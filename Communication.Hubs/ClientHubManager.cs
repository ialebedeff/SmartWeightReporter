using DynamicData.Binding;
using Entities;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Communication.Hubs
{
    /// <summary>
    /// Менеджер SignalR клиентов
    /// </summary>
    public class ClientHubManager : ReactiveObject
    {
        #region Instance
        private static Lazy<ClientHubManager> _instance = new Lazy<ClientHubManager>(new ClientHubManager());
        public static ClientHubManager Instance => _instance.Value;
        #endregion
        /// <summary>
        /// Клиенты
        /// </summary>
        public ObservableCollection<HubClient> Clients { get; set; } = new();
        /// <summary>
        /// Добавить нового клиента
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="user"></param>
        /// <param name="factory"></param>
        public void AddClient(string connectionId, User user, Factory? factory = null)
        {
            Clients.Add(new HubClient(connectionId, user, factory));
            this.RaisePropertyChanged(nameof(Clients));
        }
        /// <summary>
        /// Удалить клиента из коллекции
        /// </summary>
        /// <param name="connectionId"></param>
        public void RemoveClient(string connectionId)
        {
            if (GetClient(connectionId) is HubClient client)
            {
                Clients.Remove(client);
                this.RaisePropertyChanged(nameof(Clients));
            }
        }
        public void RemoveClient(Factory factory)
        {
            if (GetClient(factory) is HubClient hubClient)
            {
                Clients.Remove(hubClient);
                this.RaisePropertyChanged(nameof(Clients));
            }
        }
        public void RemoveClient(HubClient? hubClient)
        {
            if (hubClient is not null)
            {
                Clients.Remove(hubClient);
                this.RaisePropertyChanged(nameof(Clients));
            }
        }
        /// <summary>
        /// Получить клиента по идентификатору подключения
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public HubClient? GetClient(string connectionId)
            => Clients.FirstOrDefault(client => client.ConnectionId.Equals(connectionId));
        /// <summary>
        /// Получить клиента по аккаунту
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public HubClient? GetClient(User? user)
            => user is null ? null : Clients.FirstOrDefault(client => client.User.Id == user.Id);
        /// <summary>
        /// Получить клиента по производству
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public HubClient? GetClient(Factory? factory)
            => factory is null ? null : Clients.FirstOrDefault(client => client.Factory?.Id == factory.Id);
        /// <summary>
        /// Индексатор. Получить клиента по производству
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public HubClient? this[Factory? factory] 
            => Clients.FirstOrDefault(client => client.Factory?.Id == factory?.Id);
        /// <summary>
        /// Существует ли пользователь в коллекции
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public bool IsExists(Factory factory)
            => Clients.Any(client => client.Factory?.Id == factory.Id);
    }
}