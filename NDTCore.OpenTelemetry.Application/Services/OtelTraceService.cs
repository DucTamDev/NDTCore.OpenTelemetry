using NDTCore.OpenTelemetry.Contact.Interfaces.AppServices;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using System.Diagnostics;

namespace NDTCore.OpenTelemetry.Application.Services
{
    public class OtelTraceService : IOtelTraceService
    {
        private readonly IProductApi _productApi;
        public OtelTraceService(IProductApi productApi)
        {
            _productApi = productApi;
        }
        public void TestTrace()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ProductDto>> TraceCallService()
        {
            Activity.Current?.AddBaggage("userId", "0012");

            return await _productApi.GetAllProduct();
        }
    }
}
