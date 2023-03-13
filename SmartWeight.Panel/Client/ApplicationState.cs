using DynamicData.Binding;
using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using ReactiveUI;

namespace SmartWeight.Panel.Client
{
    public class ApplicationState : ReactiveObject
    {
        private Factory? _currentFactory;
        public Factory? CurrentFactory
        {
            get { return _currentFactory; }
            set { this.RaiseAndSetIfChanged(ref _currentFactory, value); }
        }
        
        private User? _currentUser;
        public User? CurrentUser
        {
            get { return _currentUser; }
            set { this.RaiseAndSetIfChanged(ref _currentUser, value); }
        }

        private AuthenticationState? _authenticationState;
        public AuthenticationState? AuthenticationState
        {
            get { return _authenticationState; }
            set { this.RaiseAndSetIfChanged(ref _authenticationState, value); }
        }

        private ObservableCollectionExtended<HubClient> _connectedClients = new();
        public ObservableCollectionExtended<HubClient> ConnectedClients
        {
            get { return _connectedClients; }
            set { this.RaiseAndSetIfChanged(ref _connectedClients, value); }
        }

        public void AddClient(HubClient hubClient)
        {
            if (hubClient.IsFactoryUser)
            {
                ConnectedClients.Add(hubClient);
                this.RaisePropertyChanged(nameof(ConnectedClients));
                this.RaisePropertyChanged(nameof(IsCurrentFactoryOnline));
            }
        }
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
