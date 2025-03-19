using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contract.Instrumentations;
using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product.Dtos;
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
            var stopwatch = Stopwatch.StartNew();

            using Activity? activity = _instrumentation.Tracing.StartActivity(typeof(ProductController), nameof(GetProductAllAsync));
            _instrumentation.Metrics.RecordFunctionCall(nameof(GetProductAllAsync));

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
                stopwatch.Stop();
                var duration = stopwatch.Elapsed.TotalMilliseconds;

                _instrumentation.Metrics.RecordExecutionTime(nameof(GetProductAllAsync), duration);
                _instrumentation.Tracing.LogCompletion(activity, duration, "Fetching all products - Completed");
            }
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();

            using Activity? activity = _instrumentation.Tracing.StartActivity(typeof(ProductController), nameof(GetProductByIdAsync), new Dictionary<string, object>
            {
                { "product.id", id },
                { "product.type", "integer" }
            });

            _instrumentation.Metrics.RecordFunctionCall(nameof(GetProductByIdAsync));

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
                stopwatch.Stop();
                var duration = stopwatch.Elapsed.TotalMilliseconds;

                _instrumentation.Metrics.RecordExecutionTime(nameof(GetProductByIdAsync), duration);
                _instrumentation.Tracing.LogCompletion(activity, duration, $"Fetching product by ID: {id} - Completed");
            }
        }
    }
}
