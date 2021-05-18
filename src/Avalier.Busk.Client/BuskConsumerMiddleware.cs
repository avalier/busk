using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Avalier.Busk
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/extensibility?view=aspnetcore-5.0
    public class BuskConsumerMiddleware
    {
        private readonly RequestDelegate _next;

        public BuskConsumerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context, 
            IDispatcher dispatcher,
            ILogger<BuskConsumerMiddleware> logger
        )
        {
            //*/
            if (context.Request.Path == $"/{Magic.VirtualPath.Consume}")
            {
                var cloudEvent = await context.Request.ReadCloudEventAsync();
                logger.LogInformation("Busk - Consuming event {Topic}", cloudEvent.Type);
                await dispatcher.ExecuteAsync(cloudEvent);
                context.Response.StatusCode = 500;
                return;
            }
            //*/
            await _next(context);
        }
    }
}