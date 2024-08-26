namespace Orders.Api.Application.CreateOrder;

public class CreateOrderHandler(ILogger<CreateOrderHandler> logger)
{
    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request)
    {
        var orderId = Guid.NewGuid();

        logger.LogInformation("Creating order...");

        logger.LogInformation("Order request sent.");

        return await Task.FromResult(new CreateOrderResponse(orderId, "Pending"));
    }
}
