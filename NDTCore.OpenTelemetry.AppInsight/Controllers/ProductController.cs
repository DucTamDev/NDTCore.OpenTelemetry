using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contact.Instrumentations;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi;
using System.Diagnostics;

namespace NDTCore.OpenTelemetry.AppInsight.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductApi _productApi;
        private readonly Instrumentation _instrumentation;

        public ProductController(IProductApi productApi, Instrumentation instrumentation)
        {
            _productApi = productApi;
            _instrumentation = instrumentation;
        }

        [HttpGet("GetAllProduct")]
        public async Task<IList<ProductDto>> GetProductAllAsync()
        {
            using Activity? activity = _instrumentation.Tracing.StartActivity(typeof(ProductController), nameof(GetProductAllAsync));
            var startTime = DateTime.UtcNow;

            _instrumentation.Metrics.IncrementRequestCount("/GetAllProduct");

            try
            {
                var result = await _productApi.GetAllProductAsync();

                _instrumentation.Tracing.LogEvent(activity, "Successfully fetched all products");
                activity?.SetTag("otel.status", "OK");

                return result;
            }
            catch (Exception ex)
            {
                _instrumentation.Tracing.HandleException(activity, ex, "Error while fetching all products");
                throw;
            }
            finally
            {
                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _instrumentation.Metrics.RecordRequestDuration("/GetAllProduct", duration);
                _instrumentation.Tracing.LogCompletion(activity, startTime, "Fetching all products - Completed");
            }
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            using Activity? activity = _instrumentation.Tracing.StartActivity(typeof(ProductController), nameof(GetProductByIdAsync), new Dictionary<string, object>
        {
            { "product.id", id },
            { "product.type", "integer" }
        });
            var startTime = DateTime.UtcNow;

            _instrumentation.Metrics.IncrementRequestCount("/GetProductById");

            try
            {
                var result = await _productApi.GetProductByIdAsync(id);

                _instrumentation.Tracing.LogEvent(activity, $"Successfully fetched product by ID: {id}");
                activity?.SetTag("otel.status", "OK");

                return result;
            }
            catch (Exception ex)
            {
                _instrumentation.Tracing.HandleException(activity, ex, $"Error while fetching product by ID: {id}");
                throw;
            }
            finally
            {
                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _instrumentation.Metrics.RecordRequestDuration("/GetProductById", duration);
                _instrumentation.Tracing.LogCompletion(activity, startTime, $"Fetching product by ID: {id} - Completed");
            }
        }
    }
}
