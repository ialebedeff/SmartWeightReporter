namespace Entities
{
    public class HubClient
    {
        public HubClient(
            string connectionId,
            User user,
            Factory? factory)
        {
            User = user;
            Factory = factory;
            ConnectionId = connectionId;
        }
        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Производство
        /// </summary>
        public Factory? Factory { get; set; }
        /// <summary>
        /// Идентификатор подключения
        /// </summary>
        public string ConnectionId { get; set; } = null!;
        /// <summary>
        /// Подключение от производства
        /// </summary>
        public bool IsFactoryUser => Factory is not null;
    }
}