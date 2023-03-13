namespace Entities
{
    /// <summary>
    /// Базовая модель
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public TKey Id { get; set; } = default!;
        /// <summary>
        /// Идентификатор в виде строки
        /// </summary>
        public string IdString => Id.ToString();
    }
}