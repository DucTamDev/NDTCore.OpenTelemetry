using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NDTCore.OpenTelemetry.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationConfigureServicesForAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
        public static IServiceCollection AddApplicationConfigureServicesForProduct(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
