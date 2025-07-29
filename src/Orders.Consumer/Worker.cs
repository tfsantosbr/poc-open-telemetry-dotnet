using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Orders.Consumer.Metrics;
using Orders.Consumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Correlation.Context;

namespace Orders.Consumer;

public class Worker : BackgroundService
{
    private const string QUEUE_NAME = "order-queue";
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly OrdersConsumerMetrics _metrics;
    private readonly ICorrelationContext _correlationContext;

    public Worker(
        ILogger<Worker> logger, ICorrelationContext correlationContext, OrdersConsumerMetrics metrics)
    {
        _logger = logger;
        _correlationContext = correlationContext;

        var factory = new ConnectionFactory()
        {
            HostName = "host.docker.internal",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: QUEUE_NAME,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _metrics = metrics;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            using var activity = _metrics.ActivitySource.StartActivity("ProcessOrderMessage");
            var watch = Stopwatch.StartNew();

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var createOrderMessage = JsonSerializer.Deserialize<CreateOrderMessage>(message);
            var correlationId = ea.BasicProperties.CorrelationId;

            _correlationContext.SetCorrelationId(correlationId);

            watch.Stop();
            activity?.Stop();

            _metrics.RecordOrderProcessingDuration(watch.ElapsedMilliseconds);

            _logger.LogInformation("Processed order message with Id: {OrderId} in {TotalMiliseconds}ms",
                createOrderMessage!.OrderId,
                watch.ElapsedMilliseconds);
        };

        _channel.BasicConsume(queue: QUEUE_NAME, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
