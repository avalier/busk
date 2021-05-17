using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;

namespace Avalier.Busk.Example.Host
{
    public static class OtlpExtensions
    {
        public static IApplicationBuilder UseOtlpResponseHeaders(this IApplicationBuilder app) {
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    var activity = System.Diagnostics.Activity.Current;
                    context.Response.Headers.Add("X-Trace-Id", new StringValues(activity.TraceId.ToString()));
                    context.Response.Headers.Add("X-Span-Id", new StringValues(activity.SpanId.ToString()));
                    return Task.FromResult(0);
                });
                await next.Invoke();
            });
            return app;
        }
    }

}
