using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Orders.Consumer;

public class Worker : BackgroundService
{
    private const string QUEUE_NAME = "order-queue";
    private readonly ILogger<Worker> _logger;
    private IConnection _connection;
    private IModel _channel;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",  // Substitua pelo endereço do seu RabbitMQ
            UserName = "guest",      // Substitua pelo seu usuário
            Password = "guest"       // Substitua pela sua senha
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

            _logger.LogInformation(" [x] Received {message}", message);
        };

        _channel.BasicConsume(queue: QUEUE_NAME, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
