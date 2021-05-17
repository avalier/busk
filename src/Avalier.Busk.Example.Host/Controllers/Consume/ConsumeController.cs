using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avalier.Busk.Example.Host.Controllers.Consume
{
    [ApiController]
    [Route("api/consume")]
    public class ConsumeController : ControllerBase
    {
        private readonly ILogger<ConsumeController> _logger;
        private readonly IDispatcher _dispatcher;

        public ConsumeController(
            ILogger<ConsumeController> logger,
            IDispatcher dispatcher
        )
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpPost("")] /* CloudEventValidator */
        public async Task<IActionResult> ByCloudEventAsync([FromBody]CloudEvent cloudEvent)
        {
            await _dispatcher.ExecuteAsync(cloudEvent);
            return this.Ok();
        }
    }
}
