using Orders.Consumer;
using Orders.Consumer.Metrics;
using Orders.Consumer.Extensions;
using Shared.Correlation.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddOpenTelemetryServices(builder.Environment, builder.Configuration);
builder.Services.AddCorrelationContext();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<OrdersConsumerMetrics>();

var host = builder.Build();

host.Run();
