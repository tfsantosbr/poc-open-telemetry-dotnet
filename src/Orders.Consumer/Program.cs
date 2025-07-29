using Orders.Consumer;
using Orders.Consumer.Metrics;
using Orders.Consumer.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddOpenTelemetryServices(builder.Environment, builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<OrdersConsumerMetrics>();

var host = builder.Build();

host.Run();
