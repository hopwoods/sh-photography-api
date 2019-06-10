using Microsoft.EntityFrameworkCore;
using Stuart_Hopwood_Photography_API.Entities;
using JetBrains.Annotations;

namespace Stuart_Hopwood_Photography_API.Data
{
    [UsedImplicitly]
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}