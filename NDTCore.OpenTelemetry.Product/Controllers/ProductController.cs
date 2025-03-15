using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;

namespace NDTCore.OpenTelemetry.Product.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        public ProductController() { }

        [HttpGet("all")]
        public async Task<IEnumerable<ProductDto>> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = new List<ProductDto>
            {
                new() { Id = 1, Name = "Product 1" },
                new() { Id = 2, Name = "Product 2" }
            };

            return await Task.FromResult(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid product ID.");
            }

            var product = new ProductDto { Id = id, Name = $"Product {id}" };

            return await Task.FromResult(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto, CancellationToken cancellationToken)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is required.");
            }

            return await Task.FromResult(CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto));
        }
    }
}
