using MrJB.MS.Common.Models;
using MrJB.MS.Common.Services;
using System.Text.Json;

namespace MrJB.MS.Consumer3;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IApplicationLifetime _lifetime;

    // services
    private readonly IConsumerService _consumerService;

    public Worker(ILogger<Worker> logger, IApplicationLifetime lifetime, IConsumerService consumerService)
    {
        _logger = logger;
        _lifetime = lifetime;
        _consumerService = consumerService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumerService.ProcessMessageAsync += ProcessMessage;
        return _consumerService.StartConsumingAsync(cancellationToken);
    }

    /// <summary>
    /// I don't normally use delegates and events
    /// </summary>
    private async Task ProcessMessage(string message, string operationId, string parentId, CancellationToken cancellationToken)
    {
        // log
        _logger.LogInformation($"Consumer 3: Received Message");

        // serialize message
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var order = JsonSerializer.Deserialize<Order>(message, serializerOptions);

        // process (produce to next service)
        _logger.LogInformation($"Final Message Received.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _consumerService.StopConsumingAsync(cancellationToken);
    }
}
