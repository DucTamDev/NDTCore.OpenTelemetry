using Microsoft.EntityFrameworkCore;

namespace NDTCore.OpenTelemetry.Infrastructure.Persistences
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
