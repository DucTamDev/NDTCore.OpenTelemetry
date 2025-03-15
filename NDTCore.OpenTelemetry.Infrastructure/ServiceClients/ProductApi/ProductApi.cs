using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using System.Diagnostics;
using System.Text.Json;

namespace NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi
{
    public class ProductApi : IProductApi
    {
        private const string _productApiUrl = "https://localhost:7227";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        public ProductApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IList<ProductDto>> GetAllProduct()
        {
            ActivitySource source = new("InsightSource");
            using (Activity activity = source.StartActivity("Infrastructure.ServiceClients", ActivityKind.Internal))
            {
                activity?.SetTag("client.name", "Product");
                activity?.SetTag("method.name", "GetAllProduct");
                activity?.AddBaggage("user.userId", "0012");
                activity?.AddBaggage("proxy.ip", "192.168.1.100");

                var httpClient = _httpClientFactory.CreateClient();
                using (var response = await httpClient.GetAsync($"{_productApiUrl}/api/product/all", HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<List<ProductDto>>(stream, _options);

                    return result ?? new List<ProductDto>();
                }
            }
        }

        public Task<ProductDto> GetProductById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
