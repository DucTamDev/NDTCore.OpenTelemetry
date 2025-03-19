using OpenTelemetry.Exporter;

namespace NDTCore.OpenTelemetry.Contract.ConfigutionSettings
{
    public class OpenTelemetrySettings
    {
        public string EndpointGrpc { get; set; } = string.Empty;
        public string EndpointHttp { get; set; } = string.Empty;
        public OtlpExportProtocol OtlpProtocol { get; set; } = OtlpExportProtocol.Grpc;

        public string GetActiveEndpoint() => OtlpProtocol == OtlpExportProtocol.Grpc ? EndpointGrpc : EndpointHttp;
    }
}
