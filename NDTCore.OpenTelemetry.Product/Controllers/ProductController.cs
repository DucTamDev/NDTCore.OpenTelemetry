using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using NDTCore.OpenTelemetry.Domain.Repositories;

namespace NDTCore.OpenTelemetry.Product.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductController(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<ProductDto>> GetAllProducts(CancellationToken cancellationToken)
        {
            var productsTemp = new List<ProductDto>
            {
                new() { Id = 1, Name = "Product temp 1" },
                new() { Id = 2, Name = "Product temp 2" }
            };

            var products = await _productRepository.GetAllAsync();

            if (products == null || !products.Any())
            {
                return productsTemp;
            }

            return _mapper.Map<List<ProductDto>>(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid product ID.");
            }

            var productsTemp = new ProductDto { Id = id, Name = $"Product temp {id}" };

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return productsTemp;
            }

            return _mapper.Map<ProductDto>(product);
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
