using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Secxndary.MicroserviceApp.Shared;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMq();
    }


    private void InitializeRabbitMq()
    {
        var hostName = _configuration[GlobalConstants.RabbitMQHost];
        var port = int.Parse(_configuration[GlobalConstants.RabbitMQPort]);

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: GlobalConstants.ExchangeName, ExchangeType.Fanout);

        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName, exchange: GlobalConstants.ExchangeName, routingKey: string.Empty);

        Console.WriteLine("--> Listening on the MessageBus...");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (moduleHandle, ea) =>
        {
            Console.WriteLine("--> Event Received!");

            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }


    private static void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }

    public override void Dispose()
    {
        Console.WriteLine("--> MessageBus Disposed");

        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }
}