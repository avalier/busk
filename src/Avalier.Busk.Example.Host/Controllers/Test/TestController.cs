using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avalier.Busk.Example.Host.Controllers.Test
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly Avalier.Busk.IClient _buskClient;

        public TestController(
            ILogger<TestController> logger,
            Avalier.Busk.IClient buskClient
        )
        {
            _logger = logger;
            _buskClient = buskClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Test()
        {
            var message = new Contracts.JobCompleted() { Description = "Hello World" };
            await _buskClient.PublishAsync(message);
            return this.Ok();
        }
        
    }
}
