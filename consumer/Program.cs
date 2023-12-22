using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Consumer.Data;
using Consumer.Services;
using Consumer.Mappers;
namespace Consumer;

class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional:true , reloadOnChange:true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();

        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddDbContext<DataContext>();
                services.AddScoped<IMessageService, MessageService>();
                services.AddScoped<IConsumerService, ConsumerService>();
                services.AddScoped<IMessageMapper, MessageMapper>();
            })
            .Build();
            
        var consumer = ActivatorUtilities.CreateInstance<ConsumerService>(host.Services);
        await consumer.ConsumeAsync(CancellationToken.None);

    }
}
