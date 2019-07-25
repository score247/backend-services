namespace Soccer.Services.Commands
{
    using System;
    using System.Reflection;
    using Fanex.Logging;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Soccer.API.Middlewares;
    using Soccer.Core;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                app.ConfigureExceptionHandler();
                app.UseHealthCheck();
                app.UseDatabase(Configuration);
                app.ConfigureSwagger();

                app.UseMvc();

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