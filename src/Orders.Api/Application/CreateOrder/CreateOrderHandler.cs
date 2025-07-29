using System.Text;
using System.Text.Json;
using Orders.Api.Metrics;
using RabbitMQ.Client;
using Shared.Correlation.Context;

namespace Orders.Api.Application.CreateOrder;

public class CreateOrderHandler(
    ILogger<CreateOrderHandler> logger, ICorrelationContext correlationContext, OrderMetrics metrics)
{
    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request)
    {
        var response = await CreateOrderRequest();

        return response;
    }

    private Task<CreateOrderResponse> CreateOrderRequest()
    {
        logger.LogInformation("Creating order request...");

        var orderId = Guid.NewGuid();

        SendMessage(new CreateOrderMessage(orderId));

        metrics.SumOrderRequest();

        logger.LogInformation("Order request completed.");

        return Task.FromResult(new CreateOrderResponse(orderId, "Pending"));
    }

    public void SendMessage(CreateOrderMessage message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "host.docker.internal",
            UserName = "guest",
            Password = "guest"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        string queueName = "order-queue";

        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        string jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var properties = channel.CreateBasicProperties();
        properties.CorrelationId = correlationContext.GetCorrelationId();

        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body);

        logger.LogInformation("Order request sent to message broker.");
    }
}