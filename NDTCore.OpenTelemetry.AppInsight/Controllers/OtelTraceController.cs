using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;

namespace NDTCore.OpenTelemetry.AppInsight.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtelTraceController : ControllerBase
    {
        internal IProductApi _productApi;
        public OtelTraceController(IProductApi productApi)
        {
            _productApi = productApi;
        }

        [Route("GetAllProduct")]
        [HttpGet]
        public async Task<IList<ProductDto>> GetProductAllAsync()
        {
            return await _productApi.GetAllProductAsync();
        }

        [Route("GetProductById/{id}")]
        [HttpGet]
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            return await _productApi.GetProductByIdAsync(id);
        }
    }
}
