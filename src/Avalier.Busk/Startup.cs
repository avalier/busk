using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using CloudNative.CloudEvents;
using Grpc.Core;
using OpenTelemetry.Exporter;
using Microsoft.Extensions.Primitives;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Avalier.Busk
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // Added for DI support //
        public ILifetimeScope Container { get; private set; }

        // Added for OpenApi support //
        public string Title => this.GetType().Assembly.GetName().Name;
        public int MajorVersion => this.GetType().Assembly.GetName().Version.Major;

        // Added for OpenTelemetry OTLP support //
        public string OtlpEndpoint => System.Environment.GetEnvironmentVariable("OTLP_ENDPOINT");

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                options.InputFormatters.Insert(0, new CloudEventJsonInputFormatter());
            });
            
            // Swagger //
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{MajorVersion}", new OpenApiInfo { Title = Title, Version = $"v{MajorVersion}" });
            });

            // OpenTelemetry //
            services.AddOpenTelemetryTracing(builder => {
                
                builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(this.Title));

                // Instrumentation //
                builder
                    .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                // Exporter - Console //
                //builder.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug);

                // Exporter - Jaeger //
                //builder.AddJaegerExporter();

                // Exporter - Zipkin //
                //builder.AddZipkinExporter(o => o.Endpoint = new Uri("https://localhost:9411/api/v2/spans"));

                // Exporter - OTLP //
                if (!string.IsNullOrEmpty(this.OtlpEndpoint)) {
                    Serilog.Log.Information($"Setting up OTLP exporter for endpoint: {this.OtlpEndpoint}");
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    builder.AddOtlpExporter(o => o.Endpoint = new Uri(this.OtlpEndpoint));
                }
            });

            /*/
            // Busk //
            services.AddBuskProducer("https://localhost:5001/api/publish");
            services.AddBuskConsumer(builder => builder
                .SetEndpoint("https://localhost:5001/api/publish")
                .SetConsumer("https://localhost:5001/api/consume")
                //.ScanAssembly(this.GetType().Assembly)
                .AddHandler<Handlers.JobCompletedHandler>()
                .CreateSubscriptions(true)
                .RegisterHandlers(true)
            );
            //*/

        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DefaultModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.Container = app.ThrowIfNull(nameof(app)).ApplicationServices.GetAutofacRoot();
            
            if (env.IsDevelopment())
            {
                Serilog.Log.Information("Development mode is enabled");

                // Exception page (for debugging) //
                Serilog.Log.Information("Adding exception page (development mode only)");
                app.UseDeveloperExceptionPage();

                // Swagger //
                Serilog.Log.Information("Adding swagger (development mode only)");
                app.UseSwagger(); // Swagger: Enable middleware to serve generated Swagger as a JSON endpoint.
                foreach (var routePrefix in new List<string>() { "", "api", "swagger" }) {
                    // Swagger: Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                    app.UseSwaggerUI(c =>
                    {
                        c.RoutePrefix = routePrefix;
                        c.SwaggerEndpoint($"/swagger/v{MajorVersion}/swagger.json", $"{Title} V{MajorVersion}");
                    });
                }
            }

            // Disabled for use in docker but assumes redirection will occur as a cross cutting concern in ingress //
            //app.UseHttpsRedirection();

            #region Security Additions

            // https://www.hanselman.com/blog/EasilyAddingSecurityHeadersToYourASPNETCoreWebAppAndGettingAnAGrade.aspx //
            app.UseXContentTypeOptions();
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.SameOrigin());
            app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());
            app.UseCsp(options => options
                .DefaultSources(s => s.Self()
                    .CustomSources("data:")
                    .CustomSources("https:")
                )
                .StyleSources(s => s.Self()
                    .CustomSources("www.google.com","platform.twitter.com","cdn.syndication.twimg.com","fonts.googleapis.com")
                    .UnsafeInline()
                )
                .ScriptSources(s => s.Self()
                    .CustomSources("www.google.com","cse.google.com","cdn.syndication.twimg.com","platform.twitter.com" )
                    .UnsafeInline()
                    .UnsafeEval()
                )
                .FrameAncestors(s => s.None())
                .BlockAllMixedContent()
            ); 

            #endregion

            app.UseOtlpResponseHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Registered after static files, to set headers for dynamic content. //
            app.UseXfo(xfo => xfo.Deny());
            app.UseRedirectValidation(); //Register this earlier if there's middleware that might redirect.
        }
    }
}
