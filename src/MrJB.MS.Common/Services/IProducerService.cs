namespace MrJB.MS.Common.Services;

public interface IProducerService
{
    Task ProduceAsync<T>(T data, string queue, string operationId, string parentId, CancellationToken cancellationToken);
}
