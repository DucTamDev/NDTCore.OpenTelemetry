using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NDTCore.OpenTelemetry.Contract.ConfigutionSettings;
using NDTCore.OpenTelemetry.Contract.Instrumentations;
using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product.Dtos;
using System.Diagnostics;
using System.Text.Json;

namespace NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi
{
    public class ProductApi : IProductApi
    {
        private readonly ILogger<ProductApi> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        private readonly Instrumentation _instrumentation;
        private readonly ProductApiSettings _productApiSettings;
        public ProductApi(ILogger<ProductApi> logger,
            IHttpClientFactory httpClientFactory,
            Instrumentation instrumentation,
            IOptions<ProductApiSettings> productApiSetting)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _instrumentation = instrumentation;
            _productApiSettings = productApiSetting.Value;
        }

        public async Task<IList<ProductDto>> GetAllProductAsync()
        {
            _logger.LogInformation("[ProductApi] Fetching all products from {Url}", _productApiSettings.BaseUrl);

            using (Activity? activity = _instrumentation.Tracing.StartActivity(typeof(ProductApi), nameof(GetAllProductAsync)))
            {
                activity?.SetTag("class.name", nameof(ProductApi));
                activity?.SetTag("class.method", nameof(GetAllProductAsync));
                activity?.SetTag("request.endpoint", $"{_productApiSettings.BaseUrl}/api/product/all");

                activity?.AddBaggage("user.userId", "0012");
                activity?.AddBaggage("proxy.ip", "192.168.1.100");

                activity?.AddEvent(new ActivityEvent("Fetching all products"));

                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    using var response = await httpClient.GetAsync($"{_productApiSettings.BaseUrl}/api/product/all", HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<List<ProductDto>>(stream, _options);

                    activity?.AddEvent(new ActivityEvent("Successfully fetched product list"));

                    _logger.LogInformation("[ProductApi] Successfully fetched {Count} products", result?.Count ?? 0);

                    return result ?? new List<ProductDto>();
                }
                catch (Exception ex)
                {
                    activity?.AddEvent(new ActivityEvent("Error fetching products",
                        tags: new ActivityTagsCollection { { "error.message", ex.Message } }
                    ));

                    _logger.LogError(ex, "[ProductApi] Error fetching products from {Url}", _productApiSettings.BaseUrl);

                    return new List<ProductDto>();
                }
            }
        }


        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("[ProductApi] Fetching product by ID: {ProductId}", id);

            using (Activity? activity = _instrumentation.Tracing.StartActivity(typeof(ProductApi), nameof(GetProductByIdAsync)))
            {
                activity?.SetTag("class.name", nameof(ProductApi));
                activity?.SetTag("class.method", nameof(GetProductByIdAsync));
                activity?.SetTag("request.endpoint", $"{_productApiSettings.BaseUrl}/api/product/{id}");
                activity?.AddBaggage("user.userId", "0012");

                activity?.AddEvent(new ActivityEvent("Fetching product by ID",
                    tags: new ActivityTagsCollection
                    {
                        { "product.id", id.ToString() },
                        { "class.name", nameof(ProductApi) },
                        { "request.url", $"{_productApiSettings.BaseUrl}/api/product/{id}" },
                        { "timestamp", DateTime.UtcNow.ToString("o") }
                    }
                ));

                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    using var response = await httpClient.GetAsync($"{_productApiSettings.BaseUrl}/api/product/{id}", HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("[ProductApi] Product {ProductId} not found", id);
                        return null;
                    }

                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<ProductDto>(stream, _options);

                    activity?.AddEvent(new ActivityEvent($"Successfully fetched product {id}"));
                    _logger.LogInformation("[ProductApi] Successfully fetched product {ProductId}", id);

                    return result;
                }
                catch (Exception ex)
                {
                    activity?.AddEvent(new ActivityEvent("Error fetching product",
                        tags: new ActivityTagsCollection { { "error.message", ex.Message } }
                    ));

                    _logger.LogError(ex, "[ProductApi] Error fetching product {ProductId}", id);

                    return null;
                }
            }
        }
    }
}
