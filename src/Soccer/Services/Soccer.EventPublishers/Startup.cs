﻿using JsonNet.ContractResolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Soccer.EventPublishers._Shared.Middlewares;
using Soccer.EventPublishers.Hubs;

namespace Soccer.EventPublishers
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
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new PrivateSetterContractResolver()
            };

            services.AddSignalR();
            services.AddLogging(Configuration);
            services.AddRabbitMq(Configuration);
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            //TODO: Harrison, please review it, don't know why you did it
            services.AddSingleton(services.BuildServiceProvider());
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            services.AddControllers();
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            app.ConfigureExceptionHandler();
            app.UseRabbitMq(applicationLifetime);
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SoccerHub>("/hubs/soccerhub");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    }
}