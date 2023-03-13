using Entities;

namespace Communication.Configurator
{
    public interface IMessageFactory<TParam, TResult>
        where TParam : class
        where TResult : class
    {
        public Message<TResult> CreateMessage(Factory to, TParam param);
    }
}