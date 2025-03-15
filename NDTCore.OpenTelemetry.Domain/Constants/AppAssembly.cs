using System.Reflection;

namespace NDTCore.OpenTelemetry.Domain.Constants
{
    public static class AppAssembly
    {
        public static readonly string SERVICE_NAME = Assembly.GetEntryAssembly()?.GetName().Name ?? "NDTCore.OpenTelemetry";

        public static readonly string SERVICE_VERSION = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0.0";
    }
}
