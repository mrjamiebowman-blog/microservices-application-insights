namespace MrJB.MS.Common.Services;

public interface IProducerService
{
    Task ProduceAsync(Object message, string queue, string operationId, string parentId, CancellationToken cancellationToken);
}
