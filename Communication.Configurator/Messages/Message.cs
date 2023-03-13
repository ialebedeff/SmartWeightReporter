using Entities;

namespace Communication.Configurator
{
    public class Message
    {
        /// <summary>
        /// При формировании запроса сервером
        /// </summary>
        /// <param name="data"></param>
        /// <param name="connectionId"></param>
        public Message(
            Factory to,
            string connectionId)
        {
            RequestId = Guid.NewGuid();
            ConnectionId = connectionId;
            To = to;
        }
        /// <summary>
        /// При формировании ответа клиентом
        /// </summary>
        /// <param name="data"></param>
        /// <param name="requestId"></param>
        /// <param name="connectionId"></param>
        public Message(
            Guid requestId, 
            string connectionId)
        {
            ConnectionId = connectionId;
            RequestId = requestId;
        }
        /// <summary>
        /// Идентификатор подключения
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        public Guid RequestId { get; set; }
        /// <summary>
        /// Кому отправляется сообщение
        /// </summary>
        public Factory To { get; set; }
    }
    public class Message<TData>
    { 
        /// <summary>
        /// При формировании запроса сервером
        /// </summary>
        /// <param name="data"></param>
        /// <param name="connectionId"></param>
        public Message(
            Factory to,
            TData data, 
            string connectionId) 
        {
            To = to;
            RequestId = Guid.NewGuid();
            Data = data;
            ConnectionId = connectionId;    
        }
        /// <summary>
        /// При формировании ответа клиентом
        /// </summary>
        /// <param name="data"></param>
        /// <param name="requestId"></param>
        /// <param name="connectionId"></param>
        public Message(TData data, Guid requestId, string connectionId)
        {
            ConnectionId = connectionId;
            RequestId = requestId;
            Data = data;
        }
        /// <summary>
        /// При сериализации класса 
        /// </summary>
        public Message() { }
        /// <summary>
        /// Идентификатор подключения
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        public Guid RequestId { get; set; }
        /// <summary>
        /// Полезная нагрузка
        /// </summary>
        public TData Data { get; set; }
        public static Message<TResult> CreateResponse<TParam, TResult>(
            Message<TParam> serverMessage,
            TResult result)
        {
            return new Message<TResult>(result, serverMessage.RequestId, serverMessage.ConnectionId);
        }
        /// <summary>
        /// Кому отправляется сообщение
        /// </summary>
        public Factory To { get; set; }
    }
}