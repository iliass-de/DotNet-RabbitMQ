
namespace Consumer.Services;

public interface IConsumerService
{
    Task ConsumeAsync(CancellationToken stoppingToken);
}
