using Microsoft.EntityFrameworkCore;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}