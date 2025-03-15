using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;

namespace NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product
{
    public interface IProductApi
    {
        public Task<ProductDto> GetProductById(int id);
        public Task<IList<ProductDto>> GetAllProduct();
    }
}
