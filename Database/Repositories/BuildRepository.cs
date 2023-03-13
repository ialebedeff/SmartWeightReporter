using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public class BuildRepository : RepositoryBase<Build, int>
    {
        public BuildRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public string? GetFilePath(int buildNumber, int fileId)
        {
            return Context.Builds
                .Include(build => build.Binaries)
                .FirstOrDefault(build => build.Id == buildNumber)?
                .Binaries.FirstOrDefault(binary => binary.Id == fileId)?
                .AbsolutePath;
        }

        public Task<List<Build>> GetAllBuildsAsync()
            => Context.Builds
                .Include(build => build.Binaries)
                .ToListAsync();
    }
}
