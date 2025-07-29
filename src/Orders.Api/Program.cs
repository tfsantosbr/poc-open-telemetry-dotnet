using Microsoft.AspNetCore.Mvc;
using Orders.Api.Application.CreateOrder;
using Orders.Api.Metrics;
using Orders.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Application Dependencies

builder.AddOpenTelemetryServices(builder.Environment);
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddSingleton<OrderMetrics>();

var app = builder.Build();

app.MapPost("/orders", async ([FromBody] CreateOrderRequest request, CreateOrderHandler handler) =>
{
    var response = await handler.Handle(request);

    return Results.Accepted($"/orders/{response.OrderId}", response);
});

app.Run();
