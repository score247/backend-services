using System;
using System.Reflection;
using Fanex.Logging;
using MediatR;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Soccer.API._Shared.Formatters;
using Soccer.API.Shared.Middlewares;
using Soccer.Database;

namespace Soccer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

#pragma warning disable S3902 // "Assembly.GetExecutingAssembly" should not be called

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddLogging(Configuration);
                services.AddSettings(Configuration);
                services.AddMediatR(Assembly.GetExecutingAssembly());
                services.AddServices();
                services.AddHealthCheck();
                services.AddSwagger();
                services
                    .AddMvc()
                    .AddMvcOptions(option =>
                    {
                        option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Instance));
                        option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Instance));
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }
        }

#pragma warning restore S3902 // "Assembly.GetExecutingAssembly" should not be called

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                app.UseStaticFiles();
                app.ConfigureExceptionHandler();
                app.UseHealthCheck();
                app.UseDatabase(Configuration);
                app.ConfigureSwagger();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });

                SqlMappers.RegisterJsonTypeHandlers();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }

            LogService.Info("LiveScore API has been started.");
        }
    }
}