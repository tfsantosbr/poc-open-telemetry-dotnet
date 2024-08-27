using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orders.Consumer;
using Orders.Consumer.Metrics;
using OpenTelemetry.Metrics;
using Serilog;

const string serviceName = "orders-consumer";

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<OrdersConsumerMetrics>();

// SERILOG ================================================================================================

builder.Services.AddSerilog((services, logger) => logger
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = "http://host.docker.internal:4317";
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName
        };
    })
);

// ========================================================================================================

// OPEN TELEMETRY =========================================================================================


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(OrdersConsumerMetrics.ActivitySourceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://host.docker.internal:4317")
        )
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddMeter(OrdersConsumerMetrics.MeterName)
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProcessInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://host.docker.internal:4317")
        )
    );

// =======================================================================================================

var host = builder.Build();

host.Run();
