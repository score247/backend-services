namespace Soccer.Services.EventPublishers
{
    using System;
    using GreenPipes.Configurators;
    using JsonNet.ContractResolvers;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Soccer.Core.Shared.Configurations;
    using Soccer.EventPublishers.Matches;
    using Soccer.EventPublishers.Matches.Hubs;
    using Soccer.EventPublishers.Odds;
    using Soccer.EventPublishers.Shared.Middlewares;
    using Soccer.EventPublishers.Teams.Hubs;

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
            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    ContractResolver = new PrivateSetterContractResolver()
                };
            };
            services.AddSignalR();
            services.AddLogging(Configuration);
            services.AddRabbitMq(Configuration);
            services.AddSingleton(services.BuildServiceProvider());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.ConfigureExceptionHandler();

            app.UseSignalR(routes =>
            {
                routes.MapHub<MatchEventHub>("/hubs/Soccer/MatchEventHub");
                routes.MapHub<TeamStatisticHub>("/hubs/Soccer/TeamStatisticHub");
                routes.MapHub<OddsEventHub>("/hubs/Soccer/OddsEventHub");
            });

            app.UseRabbitMq(applicationLifetime);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}