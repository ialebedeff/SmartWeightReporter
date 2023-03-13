using Database.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories
{
    public class RepositoryBase<Entity, TKey> : Repository<Entity, TKey>
        where Entity : EntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        public RepositoryBase(ApplicationContext applicationContext) 
            => Context = applicationContext;
        public ApplicationContext Context { get; set; }
        public ValueTask<Entity?> GetByIdAsync(TKey id)
        {
            return Context
                .Set<Entity>()
                .FindAsync(id);
        }
        public Task<List<Entity>> GetAllAsync()
        {
            return Context
                .Set<Entity>()
                .ToListAsync();
        }
        public async Task<OperationResult> AddRangeAsync(IEnumerable<Entity> entities)
        {
            await Context
                .Set<Entity>()
                .AddRangeAsync(entities);

            return await SaveChangesAsync();
        }
        public async Task<OperationResult<Entity>> Add(Entity entity)
        { 
            var entry = await Context
                .Set<Entity>()
                .AddAsync(entity);

            var result = await SaveChangesAsync();

            if (result.Succeed)
            {
                return OperationResult<Entity>.Ok(entry.Entity);
            }

            return OperationResult<Entity>.Failed();
        }
        public async Task<OperationResult<Entity>> UpdateAsync(Entity entity)
        {
            var entry = Context.Set<Entity>().Update(entity);
            var result = await SaveChangesAsync();

            if (result.Succeed)
            {
                return OperationResult<Entity>.Ok(entry.Entity);
            }

            return OperationResult<Entity>.Failed();
        }
        public async Task<OperationResult> DeleteAsync(Entity entity)
        {
            Context.Set<Entity>().Remove(entity);
            var result = await SaveChangesAsync();

            if (result.Succeed)
            {
                return OperationResult.Ok();
            }

            return OperationResult.Failed();
        }
        public async Task<OperationResult> SaveChangesAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch (Exception ex) { return OperationResult.Failed(ex.Message); }
        }
    }
}
