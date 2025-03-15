using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;

namespace NDTCore.OpenTelemetry.Contact.Interfaces.AppServices
{
    public interface IOtelTraceService
    {
        void TestTrace();

        Task<IList<ProductDto>> TraceCallService();
    }
}
