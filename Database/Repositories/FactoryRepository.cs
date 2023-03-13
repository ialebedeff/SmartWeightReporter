using Database.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class NoteRepository : RepositoryBase<Note, int>
    {
        public NoteRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public async Task<IEnumerable<Note>> GetNotesAsync(int factoryId, int skip, int take)
        {
            return (await Context.Notes
                .Include(note => note.User)
                .Where(note => note.FactoryId == factoryId)
                .OrderByDescending(note => note.Id)
                .ToListAsync())
                .Skip(skip)
                .Take(take)
                .Reverse();
        }
    }
    public class FactoryRepository : RepositoryBase<Factory, int>
    {
        public FactoryRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public Task<List<Factory>> GetAllFactoriesAsync()
            => Context.Factories.Include(factory => factory.User).ToListAsync();
        public async Task<IEnumerable<Factory>> SearchAsync(string? query)
        {
            if (query is not null)
            {
                var normalizedQuery = query.ToUpper();

                return Context.Factories
                .Include(factory => factory.User)
                .Include(factory => factory.DatabaseConnection)
                .Where(factory => factory.Title.ToUpper()
                .Contains(normalizedQuery) || (factory.User.Email != null ? factory.User.Email.ToUpper()
                .Contains(normalizedQuery) : false) || (factory.User.UserName != null ? factory.User.UserName.ToUpper()
                .Contains(normalizedQuery) : false));
            }

            return await Context.Factories
                .Include(factory => factory.User)
                .Include(factory => factory.DatabaseConnection)
                .ToListAsync();
        }

        public Task<Factory?> GetFactoryAsync(int factoryId)
            => Context.Factories
                .Include(factory => factory.User)
                .Include(factory => factory.DatabaseConnection)
                .FirstOrDefaultAsync(factory => factory.Id == factoryId);
        public IEnumerable<Factory> GetFactories(User user)
            => Context.Factories
                .Include(factory => factory.User)
                .Include(factory => factory.DatabaseConnection)
                .Where(factory => factory.User.Id == user.Id);
    }
}
