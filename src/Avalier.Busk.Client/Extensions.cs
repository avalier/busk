using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Avalier.Busk
{
    public static class Extensions
    {
        public static IServiceCollection AddBuskClient(this IServiceCollection services, string endpoint, string source = "")
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
    }
}