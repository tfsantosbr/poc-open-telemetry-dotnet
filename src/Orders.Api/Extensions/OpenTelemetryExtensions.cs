using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orders.Api.Metrics;

namespace Orders.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetryServices(this IHostApplicationBuilder builder, IHostEnvironment environment)
    {
        const string serviceName = "orders-api";
        var serviceNamespace = environment.EnvironmentName;
        var serviceInstance = Environment.MachineName;
        var otelCollectorEndpoint = "http://host.docker.internal:4317";

        // Configure OpenTelemetry resource builder
        // This sets the service name, namespace, and instance for the telemetry data

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceNamespace, serviceInstance);

        // Configure OpenTelemetry logging
        // This sets up logging with OpenTelemetry, using the resource builder and OTLP exporter

        builder.Logging.AddOpenTelemetry(loggerOptions =>
        {
            loggerOptions
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri(otelCollectorEndpoint)
                );
        });

        // Configure OpenTelemetry tracing and metrics
        // This sets up tracing and metrics with OpenTelemetry, using the resource builder and OTLP exporter

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .AddSource(OrderMetrics.ActivitySourceName)
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri(otelCollectorEndpoint)
                )
            )
            .WithMetrics(metrics => metrics
                .AddMeter(OrderMetrics.MeterName)
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri(otelCollectorEndpoint)
                )
            );
    }
}
