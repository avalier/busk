using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Enrichers.Span;

namespace Avalier.Busk
{
    [SuppressMessage("Microsoft.Design", "CA1031")]
    public static class Program
    {
        public static int Main(string[] args)
        {
            // Setup OpenTelemetry //
            System.Diagnostics.Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            System.Diagnostics.Activity.ForceDefaultIdFormat = true;
            
            // Reference Assemblies for DI //
            Assembly.Load("Avalier.Busk.Common");
            Assembly.Load("Avalier.Busk.InMemory");

            // Setup Logging (Serilog) //
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                //.WriteTo.Console(formatter: new CompactJsonFormatter())
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
                Log.Information("Stopped host");
                return 0;
            }
            catch (Exception x)
            {
                Log.Fatal(x, "Host terminated (due to exception)");
                return 1;
            }
            finally
            {

                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
