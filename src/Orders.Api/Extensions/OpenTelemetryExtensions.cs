using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orders.Api.Metrics;
using Shared.Correlation.Context;
using Shared.Correlation.OpenTelemetry.Processors;
using Shared.Correlation.OpenTelemetry.Extensions;

namespace Orders.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetryServices(
        this IHostApplicationBuilder builder, IHostEnvironment environment, IConfiguration configuration)
    {
        var serviceName = configuration.GetValue<string>("AppInformation:Name")!;
        var serviceVersion = configuration.GetValue<string>("AppInformation:Version")!;
        var otelCollectorEndpoint = configuration.GetValue<string>("OpenTelemetry:Collector:Endpoint")!;
        var serviceNamespace = environment.EnvironmentName;
        var serviceInstance = Environment.MachineName;

        // Configure OpenTelemetry resource builder
        // This sets the service name, namespace, and instance for the telemetry data

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceNamespace, serviceVersion, false, serviceInstance);

        // Configure OpenTelemetry logging
        // This sets up logging with OpenTelemetry, using the resource builder and OTLP exporter

        builder.Logging.AddOpenTelemetry(loggerOptions =>
        {
            loggerOptions
                .SetResourceBuilder(resourceBuilder)
                .AddCorrelationLogProcessor()
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
