using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(Configuration);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);
            services.AddServices(Configuration, appSettings);
            services.AddRabbitMq(Configuration);
            services.RegisterHangfire(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureExceptionHandler();
            app.UseHangfire(Configuration);
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}