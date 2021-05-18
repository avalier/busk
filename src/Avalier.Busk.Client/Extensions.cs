using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Avalier.Busk
{
    public static class Extensions
    {
        public static IApplicationBuilder UseBuskConsumer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BuskConsumerMiddleware>();
        }
        
        public static IServiceCollection AddBuskProducer(this IServiceCollection services, string endpoint, string source = "")
        {
            if (string.IsNullOrEmpty(source))
            {
                source = $"urn:{Assembly.GetCallingAssembly().GetName().Name}";
            }

            services.AddSingleton<Avalier.Busk.IClient>(serviceProvider =>
                Avalier.Busk.Client.Create(
                    (serviceProvider.GetService<ILogger<Avalier.Busk.Client>>()),
                    source, 
                    endpoint
                )
            );

            return services;
        }
        
        public static IServiceCollection AddBuskConsumer(this IServiceCollection services, Action<DispatcherBuilder> configure)
        {
            var builder = new DispatcherBuilder();
            configure?.Invoke(builder);
            services.AddBuskConsumer(builder);
            return services;
        }

        public static IServiceCollection AddBuskConsumer(this IServiceCollection services, DispatcherBuilder builder)
        {
            services.AddSingleton<IDispatcher>(serviceProvider => builder.Create(serviceProvider));
            
            // Register Handlers (if enabled) //
            if (builder.IsRegisterHandlers)
            {
                services.AddBuskHandlers(builder);
            }
            
            // Create Subscriptions (if enabled) //
            if (builder.IsCreateSubscriptions)
            {
                services.AddBuskSubscriptions(builder);
            }
            
            return services;
        }
        
        internal static IServiceCollection AddBuskHandlers(this IServiceCollection services, DispatcherBuilder builder)
        {
            foreach (var handlerType in builder.GetHandlerTypes())
            {
                services.AddTransient(handlerType);
            }
            return services;
        }
        
        internal static IServiceCollection AddBuskSubscriptions(this IServiceCollection services, DispatcherBuilder builder)
        {
            var request = new Dto.CreateSubscription()
            {
                Endpoint = Url.Combine(builder.Consumer, Magic.VirtualPath.Consume),
                Topics = builder.GetTopics()
            };
            
            using (var httpClientHandler = new HttpClientHandler() {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            })
            {
                var uri = Url.Combine(builder.Endpoint, Magic.VirtualPath.Subscription);
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var response = httpClient.PostAsJsonAsync(uri, request).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                }
            }
            return services;
        }
        
    }
}