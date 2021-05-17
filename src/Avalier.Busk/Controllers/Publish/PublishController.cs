using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avalier.Busk.Controllers.Publish
{
    [ApiController]
    [Route("api/publish")]
    public class PublishController : ControllerBase
    {
        private readonly ILogger<PublishController> _logger;
        private readonly IProvider _bus;

        public PublishController(
            ILogger<PublishController> logger,
            IProvider bus
        )
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost("")] /* CloudEventValidator */
        public async Task<IActionResult> ByCloudEventAsync([FromBody] CloudEvent cloudEvent)
        {
            //var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            // Map //
            //var cloudEvent = await HttpContext.Request.ReadCloudEventAsync();
            
            _logger.LogInformation("Publishing cloud event: {cloudEvent}", cloudEvent);
            await _bus.PublishAsync(cloudEvent);

            return this.Ok();
        }
        
    }
}
