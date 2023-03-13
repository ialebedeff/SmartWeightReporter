using Entities;

namespace Database.Interfaces
{
    public class Repository<Entity, TKey>
        where Entity : EntityBase<TKey>
        where TKey : IEquatable<TKey>
    { }
}
