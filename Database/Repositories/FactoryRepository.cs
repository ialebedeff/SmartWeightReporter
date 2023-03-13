using Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories
{
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
