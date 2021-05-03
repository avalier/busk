using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Avalier.Busk
{
    public static class Extensions
    {
        public static T ThrowIfNull<T>(this T value, string paramName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }
        
        public static IActionResult Validate(this CloudEvent cloudEvent) {
            
            var result = new BadRequestResult();

            // ce-id //
            if (string.IsNullOrEmpty(cloudEvent.Id))
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-id header was missing"
                };
            }
            
            // ce-specversion //
            if (cloudEvent.SpecVersion == CloudEventsSpecVersion.Default)
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-specversion header was missing"
                };
            }
            
            // ce-source //
            if (null == cloudEvent.Source)
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-source header was missing (or was not a valid uri)"
                };
            }
            
            // ce-source //
            if (string.IsNullOrEmpty(cloudEvent.Type))
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please ensure request is a valid cloud event (as per https://cloudevents.io/): ce-type header was missing"
                };
            }
            
            //if (cloudEvent.)
            //ce-source: "Avalier.Busk"
            //ce-type: "Avalier.Busk.CloudEventTests.CanCreateCloudEvent"
            //ce-id: "89434161-0986-4167-b478-7f89e2ea1248"   

            return null;
        }
    }
}