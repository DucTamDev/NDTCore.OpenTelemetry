using Microsoft.AspNetCore.Mvc;
using NDTCore.OpenTelemetry.Contact.Interfaces.AppServices;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;

namespace NDTCore.OpenTelemetry.AppInsight.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtelTraceController : ControllerBase
    {
        internal IOtelTraceService _otelTraceService;
        public OtelTraceController(IOtelTraceService otelTraceService)
        {
            _otelTraceService = otelTraceService;
        }

        [Route("TraceGetAllProduct")]
        [HttpGet]
        public async Task<IList<ProductDto>> GetProductAll(int id)
        {
            return await _otelTraceService.TraceCallService();
        }
    }
}
