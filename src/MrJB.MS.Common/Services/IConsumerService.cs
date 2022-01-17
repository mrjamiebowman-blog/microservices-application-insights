namespace MrJB.MS.Common.Services;
    
public interface IConsumerService
{
    Task StartConsumingAsync(CancellationToken token);
    Task StopConsumingAsync(CancellationToken token);
}
