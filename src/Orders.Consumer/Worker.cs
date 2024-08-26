using System.Text;
using System.Text.Json;
using Orders.Consumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Orders.Consumer;

public class Worker : BackgroundService
{
    private const string QUEUE_NAME = "order-queue";
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var createOrderMessage = JsonSerializer.Deserialize<CreateOrderMessage>(message);

            _logger.LogInformation("Processed order message with Id: {orderId}", createOrderMessage!.OrderId);
        };

        _channel.BasicConsume(queue: QUEUE_NAME, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
