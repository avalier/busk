using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Avalier.Busk.Controllers.Subscription
{
    [ApiController]
    [Route("api/subscription")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IProvider _bus;

        public SubscriptionController(
            ILogger<SubscriptionController> logger,
            IProvider bus
        )
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] Dto.CreateSubscription createSubscription)
        {
            foreach (var topic in createSubscription.Topics)
            {
                await _bus.SubscribeAsync(topic, createSubscription.Endpoint);
            }
            return this.Ok();
        }
        
        [HttpPut("{type}/{endpoint}")]
        public async Task<IActionResult> Put(string type, string endpoint)
        {
            await _bus.SubscribeAsync(type, endpoint);
            return this.Ok();
        }

        [HttpDelete("{type}/{endpoint}")]
        public async Task<IActionResult> Delete(string type, string endpoint)
        {
            
            await _bus.SubscribeAsync(type, endpoint);
            return this.Ok();
        }
    }
}
