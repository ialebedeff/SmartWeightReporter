using DynamicData.Binding;
using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using ReactiveUI;

namespace SmartWeight.Panel.Client
{
    public class ApplicationState : ReactiveObject
    {
        private Factory? _currentFactory;
        /// <summary>
        /// Текущее производство
        /// </summary>
        public Factory? CurrentFactory
        {
            get { return _currentFactory; }
            set { this.RaiseAndSetIfChanged(ref _currentFactory, value); }
        }
        private User? _currentUser;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public User? CurrentUser
        {
            get { return _currentUser; }
            set { this.RaiseAndSetIfChanged(ref _currentUser, value); }
        }
        private AuthenticationState? _authenticationState;
        /// <summary>
        /// Состояние аутентификации
        /// </summary>
        public AuthenticationState? AuthenticationState
        {
            get { return _authenticationState; }
            set { this.RaiseAndSetIfChanged(ref _authenticationState, value); }
        }
        private ObservableCollectionExtended<HubClient> _connectedClients = new();
        /// <summary>
        /// Подключенные производства 
        /// текущего аккаунта
        /// </summary>
        public ObservableCollectionExtended<HubClient> ConnectedClients
        {
            get { return _connectedClients; }
            set { this.RaiseAndSetIfChanged(ref _connectedClients, value); }
        }
        /// <summary>
        /// Добавить клиента / производство к подключенным клиентам
        /// </summary>
        /// <param name="hubClient"></param>
        public void AddClient(HubClient hubClient)
        {
            if (hubClient.IsFactoryUser)
            {
                ConnectedClients.Add(hubClient);
                this.RaisePropertyChanged(nameof(ConnectedClients));
                this.RaisePropertyChanged(nameof(IsCurrentFactoryOnline));
            }
        }
        /// <summary>
        /// Удалить клиента / производство из подключенных клиентов
        /// </summary>
        /// <param name="hubClient"></param>
        public void RemoveClient(HubClient hubClient)
        {
            if (hubClient.IsFactoryUser)
            {
                var user = ConnectedClients
                    .FirstOrDefault(client => client.ConnectionId == hubClient.ConnectionId);
                ConnectedClients.Remove(user);
                this.RaisePropertyChanged(nameof(ConnectedClients));
                this.RaisePropertyChanged(nameof(IsCurrentFactoryOnline));
            }
        }
        /// <summary>
        /// Свойство отображает состояние в 
        /// сети текущего производства
        /// </summary>
        public bool IsCurrentFactoryOnline
        {
            get 
            {
                if (CurrentFactory is null) return false;
                if (ConnectedClients is null) return false;
                return ConnectedClients.Any(client => client.Factory?.Id == CurrentFactory.Id);
            }
        } 
    }
}
