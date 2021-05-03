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
        private readonly Avalier.Busk.IClient _buskClient;

        public PublishController(
            ILogger<PublishController> logger,
            IProvider bus,
            Avalier.Busk.IClient buskClient
        )
        {
            _logger = logger;
            _bus = bus;
            _buskClient = buskClient;
        }

        [HttpPost("")] /* CloudEventValidator */
        public async Task<IActionResult> ByCloudEventAsync()
        {
            //var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            // Map //
            var cloudEvent = await HttpContext.Request.ReadCloudEventAsync();

            _logger.LogInformation("Publishing cloud event: {cloudEvent}", cloudEvent);
            await _bus.PublishAsync(cloudEvent);

            return this.Ok();
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var cloudEvent = new CloudEvent(
                "Avalier.Busk.CloudEventTests.CanCreateCloudEvent", 
                new Uri("https://avalier.io/busk")
            )
            {
                DataContentType = new ContentType(MediaTypeNames.Application.Json),
                Data = "{ \"Description\": \"Hello World\" }"
            };
            
            // Map //
            _logger.LogInformation("Publishing cloud event: {cloudEvent}", cloudEvent);
            await _bus.PublishAsync(cloudEvent);

            return this.Ok();
        }
        
        [HttpGet("test2")]
        public async Task<IActionResult> Test2()
        {
            var message = new Contracts.JobCompleted() { Description = "Hello World" };
            await _buskClient.PublishAsync(message);
            return this.Ok();
        }
        
    }
}
