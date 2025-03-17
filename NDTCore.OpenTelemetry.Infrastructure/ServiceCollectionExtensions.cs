using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Domain.Constants;
using NDTCore.OpenTelemetry.Infrastructure.Persistences;
using NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NDTCore.OpenTelemetry.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureConfigureServicesForAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackOpenTelemetry(configuration);

            services.AddHttpClient();
            services.AddScoped<IProductApi, ProductApi>();

            return services;
        }

        public static IServiceCollection AddInfrastructureConfigureServicesForProduct(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackOpenTelemetry(configuration);
            services.AddStackDatabase(configuration);

            return services;
        }

        public static IServiceCollection AddStackDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                });
            });

            return services;
        }

        public static IServiceCollection AddStackOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var resource = ResourceBuilder
                            .CreateDefault()
                            .AddContainerDetector()
                            .AddHostDetector()
                            .AddService(OtelConstants.APP_OTEL_RESOURCE_SERVICE_NAME,
                                        OtelConstants.APP_OTEL_RESOURCE_SERVICE_NAME,
                                        OtelConstants.APP_OTEL_RESOURCE_SERVICE_VERSION);

            var endpointExporter = configuration.GetValue<string>("OtelExporter:Endpoint");

            var exporter = new OtlpExporterOptions
            {
                Endpoint = new Uri(string.IsNullOrEmpty(endpointExporter)
                                ? OtelConstants.OTEL_EXPORTER_OTLP_GRPC_ENDPOINT
                                : endpointExporter),
                Protocol = OtlpExportProtocol.Grpc
            };

            services.AddLogging(logging => logging
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddOpenTelemetry(options => options
                        .SetResourceBuilder(resource)
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = exporter.Endpoint;
                            options.Protocol = exporter.Protocol;
                        })
                    )
            );

            services.AddOpenTelemetry()
                    .WithTracing(tracerBuilder => tracerBuilder
                        .SetResourceBuilder(resource)
                        .AddSource(OtelConstants.APP_OTEL_RESOURCE_SERVICE_NAME)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = exporter.Endpoint;
                            options.Protocol = exporter.Protocol;
                        })
                    );

            services.AddOpenTelemetry()
                    .WithMetrics(metricBuilder => metricBuilder
                        .SetResourceBuilder(resource)
                        .AddMeter(OtelConstants.APP_OTEL_RESOURCE_SERVICE_NAME)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = exporter.Endpoint;
                            options.Protocol = exporter.Protocol;
                        })
                    );

            return services;
        }
    }
}
