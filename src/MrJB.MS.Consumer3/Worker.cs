using MrJB.MS.Common.Services;

namespace MrJB.MS.Consumer3;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IApplicationLifetime _lifetime;

    private readonly IConsumerService _consumerService;

    public Worker(ILogger<Worker> logger, IApplicationLifetime lifetime, IConsumerService consumerService)
    {
        _logger = logger;
        _lifetime = lifetime;
        _consumerService = consumerService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _consumerService.StartConsumingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _consumerService.StopConsumingAsync(cancellationToken);
    }
}
