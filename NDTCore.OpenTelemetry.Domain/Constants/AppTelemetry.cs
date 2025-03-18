using System.Diagnostics;

namespace NDTCore.OpenTelemetry.Domain.Constants
{
    public static class AppTelemetry
    {
        public const string OTEL_EXPORTER_OTLP_GRPC_ENDPOINT = "http://localhost:4317";
        public const string OTEL_EXPORTER_OTLP_HTTP_ENDPOINT = "http://localhost:4318";

        public static readonly string APP_OTEL_RESOURCE_SERVICE_NAME = AppAssembly.SERVICE_NAME;
        public static readonly string APP_OTEL_RESOURCE_SERVICE_VERSION = AppAssembly.SERVICE_VERSION;
    }
}
