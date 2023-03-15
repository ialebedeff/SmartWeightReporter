using Communication.Configurator;
using Database;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Communication.Hubs
{
    /// <summary>
    /// SignalR хаб для обмена сообщениями
    /// </summary>
    [Authorize]
    public class MessageHub : Hub
    {
        /// <summary>
        /// Менеджер пользователей
        /// </summary>
        private readonly UserManager<User> _userManager;
        /// <summary>
        /// Менеджер ролей
        /// </summary>
        private readonly RoleManager<Role> _roleManager;
        /// <summary>
        /// Менеджер производств
        /// </summary>
        private readonly FactoryManager _factoryManager;
        /// <summary>
        /// Логер
        /// </summary>
        private readonly ILogger<MessageHub> _logger;
        /// <summary>
        /// Инициализация хаба
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="factoryManager"></param>
        public MessageHub(
            ILogger<MessageHub> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            FactoryManager factoryManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _factoryManager = factoryManager;
        }
        /// <summary>
        /// Зарегистрировать подключение клиента 
        /// как подключение от производства
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public async Task RegisterUserAsFactoryClient(Message<Factory> message)
        {
            _logger.LogInformation($"Запрос на регистрацию производства: {message.Data.Title}");
            _logger.LogInformation($"Выполняется поиск пользователя.");
            var client = ClientHubManager.Instance.GetClient(Context.ConnectionId);
            var factory = message.Data;

            if (client is not null)
            {
                // Получаем производства пользователя
                var userFactories = await _factoryManager.GetFactoriesAsync(Context.User);
                // Если у пользователя в коллекции есть отправленное
                // производство, то регистрируем его как подключение
                // от производства
                if (userFactories.Any(userFactory => userFactory.Id == factory.Id))
                { 
                    client.Factory = factory;
                    _logger.LogInformation($"Запрос на регистрацию производства: {message.Data.Title} успешно выполнен.");
                    // Отправляем уведомление об изменении
                    // количества подключенных пользователей 
                    await NotifyConnectedClientsChanged(client, true);
                }
            }
        }
        /// <summary>
        /// Получить список подключенных пользователей
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User, Admin")]
        public Task GetConnectedUsers()
            => this.Clients
                   .Client(Context.ConnectionId)
                   .SendAsync(
                    "ReceiveConnectedUsers",
                    new Message<ObservableCollection<HubClient>>(
                        ClientHubManager.Instance.Clients, 
                        Guid.NewGuid(), 
                        string.Empty));
        /// <summary>
        /// Оповещение об изменении 
        /// коллекции подключенных клиентов
        /// </summary>
        /// <param name="hubClients"></param>
        /// <returns></returns>
        private Task NotifyConnectedClientsChanged(HubClient hubClient, bool connectionState = true)
        {
            // Получаем подключения заказчика
            var consumers = ClientHubManager.Instance.Clients
                .Where(client => 
                       hubClient is not null &&  
                       !client.IsFactoryUser &&
                       client.User.Id == hubClient.User.Id)
                .Select(client => client.ConnectionId)
                .ToImmutableList();

            // Уведомляем все соединения заказчика
            // об изменении коллекции подключенных
            // производств
            return this.Clients
                .Clients(consumers)
                .SendAsync("UserConnectionStateChanged", 
                new Message<ConnectionStateChanged>(
                    new ConnectionStateChanged(hubClient, connectionState),
                    Guid.NewGuid(), string.Empty));
        }
        /// <summary>
        /// Получить состояние подключения производства
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public Task IsFactoryOnline(Message<Factory> message)
            => this.Clients
                   .Client(Context.ConnectionId)
                   .SendAsync(
                    "ReceiveUserConnectionStatus",
                    ClientHubManager.Instance.IsExists(message.Data));
        /// <summary>
        /// Отправить сообщение с SQL командой
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public Task DatabaseMessageExecute(Message<DatabaseCommand> message)
            => this.SendToClientAsync("DatabaseMessageExecute", message); 
        /// <summary>
        /// Отправить сообщение с SQL командой клиенту 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public Task CarsDatabaseMessageExecute(Message<DatabaseCommand> message)
            => this.SendToClientAsync("CarsDatabaseMessageExecute", message); //CarsDatabaseMessageExecute
        /// <summary>
        /// Отправить сообщение с результатом выполнения SQL команды
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public Task DatabaseMessageResult(Message<List<Dictionary<string, object>>> message)
            => this.Clients
                   .Client(message.ConnectionId)
                   .SendAsync("DatabaseMessageResult", message);
        /// <summary>
        /// Отправить сообщение с результатом выполнения 
        /// SQL-команды к таблице work_cars
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public Task CarsDatabaseMessageResult(Message<List<Dictionary<string, object>>> message)
            => this.Clients
                   .Client(message.ConnectionId)
                   .SendAsync("CarsDatabaseMessageResult", message);
        /// <summary>
        /// Отправить сообщение клиенту
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task SendToClientAsync<TData>(string methodName, Message<TData> message)
            where TData : class
        {
            // Получаем информацию о клиента
            // по производству
            var client = ClientHubManager.Instance[message.To];

            if (client is not null) 
            {
                message.ConnectionId = Context.ConnectionId;
                // Отправляем сообщение клиента
                await this.Clients
                    .Client(client.ConnectionId)
                    .SendAsync(methodName, message);
            }
        }
        /// <summary>
        /// Ивент подключения, добавляем пользователя в коллекцию
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            // Регистрируем новое подключение и
            // добавляем нового клиента в менеджер клиентов
            var connectionId = Context.ConnectionId;
            var user = await _userManager.GetUserAsync(this.Context.User) ?? throw new Exception();

            _logger.LogInformation($"Пользователь: {user.UserName} успешно подключен");

            ClientHubManager.Instance.AddClient(connectionId, user);
        }
        /// <summary>
        /// Ивент отключения, удаляем пользователя из коллекции
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            // При отключении клиента, удаляем информацию
            // по текущему пользователю из менеджера клиентов
            _logger.LogInformation($"Пользователь отключен: {exception}");

            var client = ClientHubManager.Instance.GetClient(Context.ConnectionId);
            ClientHubManager.Instance.RemoveClient(client);
            // Отправляем уведомление об изменении
            // количества подключенных пользователей 
            return NotifyConnectedClientsChanged(client, false);
        }
    }
}