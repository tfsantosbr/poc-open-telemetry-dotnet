using Microsoft.AspNetCore.Mvc;
using Orders.Api.Application.CreateOrder;

var builder = WebApplication.CreateBuilder(args);

// Application Dependencies
builder.Services.AddScoped<CreateOrderHandler>();

var app = builder.Build();

app.MapPost("/orders", async ([FromBody] CreateOrderRequest request, CreateOrderHandler handler) =>
{
    var response = await handler.Handle(request);

    return Results.Accepted($"/orders/{response.OrderId}", response);
});

app.Run();
