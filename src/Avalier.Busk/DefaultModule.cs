using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Avalier.Busk
{
    [ExcludeFromCodeCoverage]
    public class DefaultModule : Autofac.Module
    {
        public IConfiguration Configuration { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            // Register services by convention for assemblies... //
            var assembly = Assembly.GetEntryAssembly();
            var filter = assembly?.GetName().Name?.Split(".").FirstOrDefault() ?? "";
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith(filter, StringComparison.InvariantCulture))
                .Where(a => !a.FullName.StartsWith("Avalier.Busk.Client", StringComparison.InvariantCulture))
                .Union(new[] { assembly })
                .ToArray();

            // Automatic Registrations //
            builder.RegisterAssemblyTypes(assemblies).AsImplementedInterfaces();

            // Explicit Registrations //
            builder.RegisterType<Avalier.Busk.Provider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<Avalier.Busk.InMemory.StorageProvider>().AsImplementedInterfaces().SingleInstance();
            
        }  
        
    }
}