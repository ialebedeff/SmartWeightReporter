using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace Communication.Configurator
{
    public abstract class CommunicationConfiguratorBase
    {
        /// <summary>
        /// Настроить подключение по SignalR
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <returns></returns>
        public abstract HubConnection Configure(string url, CookieContainer? cookieContainer = null);
        /// <summary>
        /// Настроить подключение по SignalR с 
        /// дополнительным функционалом
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <returns></returns>
        public abstract HubConnection Configure(string url, Action<HubConnection> configuration);
    }
}