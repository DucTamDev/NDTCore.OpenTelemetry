using Microsoft.EntityFrameworkCore;

namespace NDTCore.OpenTelemetry.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
