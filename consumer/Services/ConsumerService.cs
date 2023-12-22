namespace Consumer.Services;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Consumer.Entities;
using Consumer.Models;

public class ConsumerService: IConsumerService
{
    private readonly IMessageService _messageService;
    private readonly IConfiguration _configuration;

    public ConsumerService(IMessageService messageService, IConfiguration configuration)
    {
        _messageService = messageService;
        _configuration = configuration;
    }

    public async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        var serviceConnection = _configuration.GetSection("ServiceConnection");
        var connectionString = serviceConnection.GetValue<string>("RabbitMQConnection");
        Console.WriteLine(connectionString);
        ConnectionFactory factory = new()
        {
            Uri = new Uri(connectionString)
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var rabbitParameters = _configuration.GetSection("RabbitMQParameters");
        var queueName = rabbitParameters.GetValue<string>("QueueName");
        var exchangeName = rabbitParameters.GetValue<string>("ExchangeName");
        var routingKey = rabbitParameters.GetValue<string>("RoutingKey");
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (ModuleHandle, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message: '{message}' has been received from the queue");
            try{
                var messageModel = JsonSerializer.Deserialize<MessageModel>(message);
                if(messageModel is null)
                    throw new ArgumentNullException(nameof(messageModel));
                messageModel.CreatedAt = DateTime.UtcNow;
                await _messageService.CreateMessageAsync(messageModel);
                Console.WriteLine($"Message: '{messageModel.Message}' has been saved");
            }
            catch(Exception ex){
                Console.WriteLine($"Message: '{message}' has not been saved to the database");
                Console.WriteLine(ex.Message);
            }
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        string consumerTag = channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        Console.ReadLine();
        channel.BasicCancel(consumerTag);
        channel.Close();
        connection.Close();
        await Task.CompletedTask;
    }
}