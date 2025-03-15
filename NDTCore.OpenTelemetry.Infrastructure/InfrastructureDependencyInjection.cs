using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Domain.Constants;
using NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NDTCore.OpenTelemetry.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackOpenTelemetry(configuration);

            services.AddHttpClient();
            services.AddScoped<IProductApi, ProductApi>();

            return services;
        }

        public static IServiceCollection AddStackOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var resource = ResourceBuilder
                            .CreateDefault()
                            .AddService(OtelConstants.APP_OTEL_RESOURCE_SERVICE_NAME,
                                        OtelConstants.APP_OTEL_RESOURCE_SERVICE_NAME,
                                        OtelConstants.APP_OTEL_RESOURCE_SERVICE_VERSION);

            services.AddLogging(logging => logging
                    .AddOpenTelemetry(options => options
                        .SetResourceBuilder(resource)
                        .AddConsoleExporter()
                        .AddOtlpExporter()
                    )
            );

            services.AddOpenTelemetry()
                    .WithTracing(tracerBuilder => tracerBuilder
                        .SetResourceBuilder(resource)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            // can get it in appsettings.json
                            options.Endpoint = new Uri(OtelConstants.OTEL_EXPORTER_OTLP_ENDPOINT);
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        })
                    );

            services.AddOpenTelemetry()
                    .WithMetrics(metricBuilder => metricBuilder
                        .SetResourceBuilder(resource)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(OtelConstants.OTEL_EXPORTER_OTLP_ENDPOINT);
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        })
                    );

            return services;
        }
    }
}
