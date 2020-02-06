using JsonNet.ContractResolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Refit;
using Score247.Shared.Middlewares;
using Soccer.NotificationPublishers._Shared.Configuration;
using Soccer.NotificationPublishers._Shared.Middlewares;
using Soccer.NotificationPublishers.Matches;

namespace Soccer.NotificationPublishers
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

            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);

            var appCenterSettings = new AppCenterSettings();
            Configuration.GetSection("PushServices:AppCenter").Bind(appCenterSettings);
            services.AddSingleton<IAppCenterSettings>(appCenterSettings);

            services.AddSingleton(RestService.For<IPushApi>(appCenterSettings.ServiceUrl));
            services.AddSingleton<IMatchNotificationPublisher, MatchNotificationPublisher>();

            services.AddCors(appSettings);
            services.AddSignalR();
            services.AddLogging(Configuration);
            services.AddRabbitMq(Configuration);
            services.AddHealthCheck();
            services.AddControllers();
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            app.AddCors();
            app.UseStaticFiles();
            app.ConfigureExceptionHandler();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRabbitMq(applicationLifetime);
            app.UseHealthCheck();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    }
}
