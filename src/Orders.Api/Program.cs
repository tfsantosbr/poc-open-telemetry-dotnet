using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orders.Api.Application.CreateOrder;
using Orders.Api.Metrics;
using Serilog;
using Serilog.Events;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

const string serviceName = "orders-api";

var builder = WebApplication.CreateBuilder(args);

// Application Dependencies

builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddSingleton<OrderMetrics>();

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
        .AddSource(OrderMetrics.ActivitySourceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://host.docker.internal:4317")
        )
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddMeter(OrderMetrics.MeterName)
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProcessInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://host.docker.internal:4317")
        )
    );

// =======================================================================================================

var app = builder.Build();

app.MapPost("/orders", async ([FromBody] CreateOrderRequest request, CreateOrderHandler handler) =>
{
    var response = await handler.Handle(request);

    return Results.Accepted($"/orders/{response.OrderId}", response);
});

app.Run();
