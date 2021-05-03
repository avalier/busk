using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Avalier.Busk
{
    public class CloudEventValidatorAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cloudEvent = await context.HttpContext.Request.ReadCloudEventAsync();
            
            // ce-specversion //
            if (true
                && string.IsNullOrEmpty(context.HttpContext.Request.Headers["ce-specversion"]) 
                && string.IsNullOrEmpty(context.HttpContext.Request.Headers["ce-cloudEventsVersion"]))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-specversion header was missing"
                };
                return;
            }
            
            // ce-id //
            if (string.IsNullOrEmpty(cloudEvent.Id))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-id header was missing"
                };
                return;
            }
            
            // ce-source //
            if (null == cloudEvent.Source)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-source header was missing (or was not a valid uri)"
                };
                return;
            }
            
            // ce-source //
            if (string.IsNullOrEmpty(cloudEvent.Type))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-type header was missing"
                };
                return;
            }
            
            await base.OnActionExecutionAsync(context, next);
        }
    }
}