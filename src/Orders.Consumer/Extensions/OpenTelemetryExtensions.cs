using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orders.Consumer.Metrics;
using Shared.Correlation.OpenTelemetry.Extensions;

namespace Orders.Consumer.Extensions;

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
                .AddSource(OrdersConsumerMetrics.ActivitySourceName)
                .SetResourceBuilder(resourceBuilder)
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri(otelCollectorEndpoint)
                )
            )
            .WithMetrics(metrics => metrics
                .AddMeter(OrdersConsumerMetrics.MeterName)
                .SetResourceBuilder(resourceBuilder)
                .AddMeter(OrdersConsumerMetrics.MeterName)
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri(otelCollectorEndpoint)
                )
            );
    }
}
