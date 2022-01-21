using Microsoft.AspNetCore.Mvc;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Models;
using MrJB.MS.Common.Services;

namespace MrJB.MS.Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // logging
        private readonly ILogger<OrderController> _logger;

        // configuration
        private readonly AzureServiceBusProducerConfiguration _azureServiceBusProducerConfiguration;

        // service
        private readonly IProducerService _producerService;

        public OrderController(ILogger<OrderController> logger, AzureServiceBusProducerConfiguration azureServiceBusProducerConfiguration, IProducerService producerService)
        {
            _logger = logger;
            _azureServiceBusProducerConfiguration = azureServiceBusProducerConfiguration;
            _producerService = producerService;
        }

        /// <summary>
        /// Hypothetical Situation: this API will receive order information from an external system.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("create")]
        public Task CreateOrderAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // create order
                var order = new Order();
                order.OrderId = new Random().Next(1, 1000);
                order.Subtotal = new Random().Next(100, 500); ;
                order.Tax = order.Subtotal * 0.07M;
                order.Total = order.Subtotal + order.Tax;

                // billing address
                order.BillingAddress = new CustomerAddress() {
                    FirstName = "Jamie",
                    LastName = "Bowman",
                    StreetAddress1 = "123 Street",
                    StreetAddress2 = "Apt #2",
                    City = "Saint Louis",
                    State = "MO",
                    Country = "USA",
                    PostalCode = "12345"
                };

                // shipping address
                order.ShippingAddress = new CustomerAddress() {
                    FirstName = "Jamie",
                    LastName = "Bowman",
                    StreetAddress1 = "123 Street",
                    StreetAddress2 = "Apt #2",
                    City = "Saint Louis",
                    State = "MO",
                    Country = "USA",
                    PostalCode = "12345"
                };

                // post to service
                _producerService.ProduceAsync(order, _azureServiceBusProducerConfiguration.QueueOrTopic, "", "", cancellationToken);

                return Task.CompletedTask;
            } catch (Exception ex) {
                // TODO: return problem details
                throw new Exception("Unable to Create Order.");
            }
        }
    }
}
