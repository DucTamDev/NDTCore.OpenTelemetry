using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product.Dtos;

namespace NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product
{
    public interface IProductApi
    {
        public Task<ProductDto?> GetProductByIdAsync(int id);
        public Task<IList<ProductDto>> GetAllProductAsync();
    }
}
