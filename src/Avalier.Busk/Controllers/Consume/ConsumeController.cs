using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avalier.Busk.Controllers.Consume
{
    [ApiController]
    [Route("api/consume")]
    public class ConsumeController : ControllerBase
    {
        private readonly ILogger<ConsumeController> _logger;

        public ConsumeController(ILogger<ConsumeController> logger)
        {
            _logger = logger;
        }

        [HttpPost(""), CloudEventValidator]
        public async Task<IActionResult> ByCloudEventAsync()
        {
            var stream = new StreamReader(HttpContext.Request.Body);
            var body = await stream.ReadToEndAsync();
            
            var cloudEvent = await HttpContext.Request.ReadCloudEventAsync();
            var json = JsonConvert.SerializeObject(cloudEvent);
            return this.Ok();
        }
    }
}
