using Microsoft.AspNetCore.Mvc;
using Orders.Api.Application.CreateOrder;
using Orders.Api.Metrics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Application Dependencies

builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddSingleton<OrderMetrics>();

// SERILOG ================================================================================================

builder.Services.AddSerilog((services, logger) => logger
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console());

// ========================================================================================================

var app = builder.Build();

app.MapPost("/orders", async ([FromBody] CreateOrderRequest request, CreateOrderHandler handler) =>
{
    var response = await handler.Handle(request);

    return Results.Accepted($"/orders/{response.OrderId}", response);
});

app.Run();
