using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using NDTCore.OpenTelemetry.Domain.Constants;
using System.Diagnostics;

namespace NDTCore.OpenTelemetry.AppInsight.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        internal IProductApi _productApi;
        public ProductController(IProductApi productApi)
        {
            _productApi = productApi;
        }

        [Route("GetAllProduct")]
        [HttpGet]
        public async Task<IList<ProductDto>> GetProductAllAsync()
        {
            using (Activity? activity = OtelConstants.ActivitySource.StartActivity(typeof(ProductController).FullName ?? nameof(ProductController)))
            {
                activity?.SetTag("class.name", nameof(ProductController));
                activity?.SetTag("class.method", nameof(GetProductAllAsync));

                return await _productApi.GetAllProductAsync();
            }
        }

        [Route("GetProductById/{id}")]
        [HttpGet]
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            using (Activity? activity = OtelConstants.ActivitySource.StartActivity(typeof(ProductController).FullName ?? nameof(ProductController)))
            {
                activity?.SetTag("class.name", nameof(ProductController));
                activity?.SetTag("class.method", nameof(GetProductByIdAsync));

                return await _productApi.GetProductByIdAsync(id);
            }
        }
    }
}
