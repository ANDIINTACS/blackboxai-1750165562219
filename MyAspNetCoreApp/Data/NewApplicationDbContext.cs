using Microsoft.EntityFrameworkCore;

namespace MyAspNetCoreApp.Data
{
    public class NewApplicationDbContext : DbContext
    {
        public NewApplicationDbContext(DbContextOptions<NewApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for your new database tables here
        public DbSet<MyAspNetCoreApp.Models.Client> Clients { get; set; }
    }
}
