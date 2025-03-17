using Microsoft.Extensions.Logging;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using System.Diagnostics;
using System.Text.Json;

namespace NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi
{
    public class ProductApi : IProductApi
    {
        private readonly ILogger<ProductApi> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;

        private const string _productApiUrl = "https://localhost:44340";
        public ProductApi(IHttpClientFactory httpClientFactory, ILogger<ProductApi> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IList<ProductDto>> GetAllProductAsync()
        {
            _logger.LogInformation("[ProductApi] Fetching all products from {Url}", _productApiUrl);

            using (Activity? activity = Activity.Current)
            {
                activity?.SetTag("client.name", "ProductApi");
                activity?.SetTag("method.name", nameof(GetAllProductAsync));
                activity?.SetTag("request.endpoint", $"{_productApiUrl}/api/product/all");

                activity?.AddBaggage("user.userId", "0012");
                activity?.AddBaggage("proxy.ip", "192.168.1.100");

                activity?.AddEvent(new ActivityEvent("Fetching all products"));

                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    using var response = await httpClient.GetAsync($"{_productApiUrl}/api/product/all", HttpCompletionOption.ResponseHeadersRead);
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

                    _logger.LogError(ex, "[ProductApi] Error fetching products from {Url}", _productApiUrl);
                    throw;
                }
            }
        }


        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("[ProductApi] Fetching product by ID: {ProductId}", id);

            using (Activity? activity = Activity.Current)
            {
                activity?.SetTag("client.name", "ProductApi");
                activity?.SetTag("method.name", nameof(GetProductByIdAsync));
                activity?.SetTag("request.endpoint", $"{_productApiUrl}/api/product/{id}");
                activity?.AddBaggage("user.userId", "0012");

                activity?.AddEvent(new ActivityEvent("Fetching product by ID",
                    tags: new ActivityTagsCollection
                    {
                        { "product.id", id.ToString() },
                        { "client.name", "ProductApi" },
                        { "request.url", $"{_productApiUrl}/api/product/{id}" },
                        { "timestamp", DateTime.UtcNow.ToString("o") }
                    }
                ));

                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    using var response = await httpClient.GetAsync($"{_productApiUrl}/api/product/{id}", HttpCompletionOption.ResponseHeadersRead);

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
                    throw;
                }
            }
        }
    }
}
