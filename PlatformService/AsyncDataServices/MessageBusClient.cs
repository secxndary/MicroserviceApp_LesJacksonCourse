using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;
using Secxndary.MicroserviceApp.Shared;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

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

        try
        {
            Console.WriteLine($"--> Trying to connect to the MessageBus... Endpoint: {hostName}:{port}");

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: GlobalConstants.ExchangeName, type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Connected to the MessageBus!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the MessageBus: {ex.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
    {
        var message = JsonSerializer.Serialize(platformPublishDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: GlobalConstants.ExchangeName,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("--> MessageBus Disposed");

        if (!_channel.IsOpen) 
            return;

        _channel.Close();
        _connection.Close();
    }

    private static void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
}