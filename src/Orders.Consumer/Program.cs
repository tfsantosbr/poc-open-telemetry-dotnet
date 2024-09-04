using Orders.Consumer;
using Orders.Consumer.Metrics;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<OrdersConsumerMetrics>();

// SERILOG ================================================================================================

builder.Services.AddSerilog((services, logger) => logger
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console());

// ========================================================================================================

var host = builder.Build();

host.Run();
