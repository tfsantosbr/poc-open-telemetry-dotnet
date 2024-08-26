var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/orders", () =>
{
    var response = new { OrderId = Guid.NewGuid(), OrderStatus = "Pending" };

    return Results.Accepted($"/orders/{response.OrderId}", response);
});

app.Run();