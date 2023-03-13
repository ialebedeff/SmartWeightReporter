using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class ApplicationContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions): base(dbContextOptions) { Database.EnsureCreated(); }
        protected override void OnModelCreating(ModelBuilder builder)
        { 
            base.OnModelCreating(builder);
        }

        public DbSet<Factory> Factories => Set<Factory>();
        public DbSet<Build> Builds => Set<Build>();
        public DbSet<Note> Notes => Set<Note>();
    }
}