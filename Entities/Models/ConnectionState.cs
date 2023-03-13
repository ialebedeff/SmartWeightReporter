using System.Data;

namespace Entities.Models
{
    public class ConnectionStateChanged
    {
        public HubClient HubClient { get; set; } = null!;
        public ConnectionState State { get; set; }
        public ConnectionStateChanged() { }
        public ConnectionStateChanged(HubClient hubClient, ConnectionState state)
        {
            HubClient = hubClient;
            State = state;
        }
        public ConnectionStateChanged(HubClient hubClient, bool connectionState) :
            this(hubClient, connectionState ? ConnectionState.Open : ConnectionState.Closed)
        { }
    }
}
