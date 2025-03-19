using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDTCore.OpenTelemetry.Contract.ConfigutionSettings;
using NDTCore.OpenTelemetry.Contract.Instrumentations;
using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product;
using NDTCore.OpenTelemetry.Domain.Constants;
using NDTCore.OpenTelemetry.Domain.Repositories;
using NDTCore.OpenTelemetry.Infrastructure.Persistences;
using NDTCore.OpenTelemetry.Infrastructure.Repositories;
using NDTCore.OpenTelemetry.Infrastructure.ServiceClients.ProductApi;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace NDTCore.OpenTelemetry.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureConfigureServicesForAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<OpenTelemetrySettings>(configuration.GetSection(nameof(OpenTelemetrySettings)));
            services.Configure<ProductApiSettings>(configuration.GetSection(nameof(ProductApiSettings)));

            services.AddAutoMapper(Assembly.Load("NDTCore.OpenTelemetry.Contract"));

            services.AddStackOpenTelemetry(configuration);

            services.AddHttpClient();
            services.AddScoped<IProductApi, ProductApi>();

            services.AddSingleton<Instrumentation>();

            return services;
        }

        public static IServiceCollection AddInfrastructureConfigureServicesForProduct(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<OpenTelemetrySettings>(configuration.GetSection(nameof(OpenTelemetrySettings)));

            services.AddAutoMapper(Assembly.Load("NDTCore.OpenTelemetry.Contract"));

            services.AddStackOpenTelemetry(configuration);
            services.AddStackDatabase(configuration);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddSingleton<Instrumentation>();
            return services;
        }

        public static IServiceCollection AddStackDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
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
                            .AddService(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME,
                                        AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME,
                                        AppTelemetry.APP_OTEL_RESOURCE_SERVICE_VERSION);

            var openTelemetryOptions = configuration
                .GetSection(nameof(OpenTelemetrySettings))
                .Get<OpenTelemetrySettings>() ?? new OpenTelemetrySettings();

            var exporter = new OtlpExporterOptions
            {
                Endpoint = new Uri(openTelemetryOptions.GetActiveEndpoint()),
                Protocol = openTelemetryOptions.OtlpProtocol
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
                        .AddSource(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.SetDbStatementForText = true;
                            options.SetDbStatementForStoredProcedure = true;
                        })
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
                        .AddMeter(AppTelemetry.APP_OTEL_RESOURCE_SERVICE_NAME)
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
