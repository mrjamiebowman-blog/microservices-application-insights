namespace MrJB.MS.Common.Services;
    
public interface IConsumerService
{
    delegate Task MessageReceivedAsync(string message, string operationId, string parentId, CancellationToken cancellationToken);
    event MessageReceivedAsync ProcessMessageAsync;

    Task StartConsumingAsync(CancellationToken token);
    Task StopConsumingAsync(CancellationToken token);
}
