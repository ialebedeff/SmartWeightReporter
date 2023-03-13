using Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories
{
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
}
