using NDTCore.OpenTelemetry.Domain.Entities;
using NDTCore.OpenTelemetry.Domain.Repositories;
using NDTCore.OpenTelemetry.Infrastructure.Persistences;

namespace NDTCore.OpenTelemetry.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
