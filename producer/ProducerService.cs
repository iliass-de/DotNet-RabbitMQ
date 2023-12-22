namespace Producer.Service;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Producer.Models;
using System.Text;
using System.Text.Json;

public static class ProducerService
{
    private static IConfiguration _configuration = GetConfiguration();
    public static async Task ProduceAsync(CancellationToken stoppingToken)
    {
        var serviceConnection = _configuration.GetSection("ServiceConnection");
        var connectionString = serviceConnection.GetValue<string>("RabbitMQConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("RabbitMQ connection string is empty");
            return;
        }
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
        Console.WriteLine("Enter a message or type exit");
        var input = Console.ReadLine();
        var message = new MessageModel();
        var body = new byte[0];
        while(input != "exit"){
            message = new MessageModel
            {
                Message = input
            };
            body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
            Console.WriteLine("Message sent");
            Console.WriteLine("Enter message");
            input = Console.ReadLine();
        }
        await Task.CompletedTask;
    }

    private static IConfiguration GetConfiguration(){
        IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional:true , reloadOnChange:true)
        .AddEnvironmentVariables()
        .Build();
        return configuration;
    }
}