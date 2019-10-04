namespace Soccer.Services.EventPublishers
{
    using JsonNet.ContractResolvers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Soccer.EventPublishers.Hubs;
    using Soccer.EventPublishers.Shared.Middlewares;

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
            services.AddControllers();
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
    }
}