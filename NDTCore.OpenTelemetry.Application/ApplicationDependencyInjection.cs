using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NDTCore.OpenTelemetry.Application.Services;
using NDTCore.OpenTelemetry.Contact.Interfaces.AppServices;

namespace NDTCore.OpenTelemetry.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOtelTraceService, OtelTraceService>();
            services.AddScoped<IOtelMetricService, OtelMetricService>();

            return services;
        }
    }
}
