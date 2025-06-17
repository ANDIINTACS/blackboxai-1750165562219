using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Models;

namespace MyAspNetCoreApp.Data
{
    public class INTSystemDbContext : DbContext
    {
        public INTSystemDbContext(DbContextOptions<INTSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> intClient { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .ToTable("intClient");
        }
    }
}
