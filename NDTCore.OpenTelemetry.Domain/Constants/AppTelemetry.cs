using System.Diagnostics;

namespace NDTCore.OpenTelemetry.Domain.Constants
{
    public static class AppTelemetry
    { 
        public static readonly string APP_OTEL_RESOURCE_SERVICE_NAME = AppAssembly.SERVICE_NAME;
        public static readonly string APP_OTEL_RESOURCE_SERVICE_VERSION = AppAssembly.SERVICE_VERSION;
    }
}
