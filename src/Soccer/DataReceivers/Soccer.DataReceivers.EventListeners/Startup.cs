namespace Soccer.DataReceivers.EventListeners
{
    using System;
    using Fanex.Caching;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Refit;
    using Sentry;
    using Soccer.Core.Shared.Configurations;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.Odds;
    using Soccer.DataProviders.SportRadar.Matches.Services;
    using Soccer.DataProviders.SportRadar.Odds;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataReceivers.EventListeners.Matches;
    using Soccer.DataReceivers.EventListeners.Shared.Configurations;
    using Soccer.DataReceivers.Odds;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            };

            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<ICacheService, CacheService>();

            RegisterLogging(services);
            RegisterRabbitMq(services);
            RegisterSportRadarDataProvider(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseStaticFiles();
            app.UseMvc();
        }

        public void RegisterLogging(IServiceCollection services)
        {
            LogManager
                   .SetDefaultLogCategory(Configuration["Fanex.Logging:DefaultCategory"])
                   .Use(new SentryLogging(new SentryEngineOptions
                   {
                       Dsn = new Dsn(Configuration["Fanex.Logging:SentryUrl"])
                   }));

            services.AddSingleton(Logger.Log);
        }

        private void RegisterRabbitMq(IServiceCollection services)
        {
            var messageQueueSettings = new MessageQueueSettings();
            Configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(x =>
            {
                x.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(
                  cfg =>
                  {
                      cfg.Host(
                          messageQueueSettings.Host,
                          messageQueueSettings.Port,
                           messageQueueSettings.VirtualHost, h =>
                           {
                               h.Username(messageQueueSettings.Username);
                               h.Password(messageQueueSettings.Password);
                           });
                  }));
            });
        }

        private void RegisterSportRadarDataProvider(IServiceCollection services)
        {
            var sportRadarDataProviderSettings = new SportRadarSettings();
            Configuration
                .GetSection("DataProviders:SportRadar")
                .Bind(sportRadarDataProviderSettings);

            services.AddSingleton<ISportRadarSettings>(sportRadarDataProviderSettings);
            services.AddSingleton(RestService.For<IMatchApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IMatchService, MatchService>();
            services.AddSingleton<IMatchEventListenerService, MatchEventListenerService>();

            services.AddScoped<IOddsMessagePublisher, OddsMessagePublisher>();
            services.AddScoped<IOddsService, OddsService>();
            services.AddSingleton(RestService.For<IOddsApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);

            services.AddHostedService<MatchEventListener>();
        }
    }
}