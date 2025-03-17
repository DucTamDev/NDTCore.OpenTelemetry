using Microsoft.EntityFrameworkCore;
using NDTCore.OpenTelemetry.Domain.Entities;

namespace NDTCore.OpenTelemetry.Infrastructure.Persistences
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().ToTable("Products");
        }
    }
}
