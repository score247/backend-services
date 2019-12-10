using System;
using System.Collections.Generic;
using System.Net.Http;
using Fanex.Caching;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Refit;
using Score247.Shared;
using Sentry;
using Soccer.Cache.Leagues;
using Soccer.Core._Shared.Helpers;
using Soccer.Core.Shared.Configurations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Internal._Share.Configurations;
using Soccer.DataProviders.Internal.Leagues.Services;
using Soccer.DataProviders.Internal.Share.Helpers;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataProviders.SportRadar.Leagues.Services;
using Soccer.DataProviders.SportRadar.Matches.Services;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataReceivers.EventListeners.Matches;
using Soccer.DataReceivers.EventListeners.Shared.Configurations;

namespace Soccer.DataReceivers.EventListeners
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
            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            };

            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);
            var internalDataProviderSettings = new InternalProviderSettings();
            Configuration.GetSection("DataProviders:Internal").Bind(internalDataProviderSettings);

            var sportRadarDataProviderSettings = new SportRadarSettings();
            Configuration.GetSection("DataProviders:SportRadar").Bind(sportRadarDataProviderSettings);

            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ILeagueCache, LeagueCache>();
            services.AddSingleton<ILeagueCache, LeagueCache>();
            var cryptoHelper = new CryptographyHelper();
            services.AddSingleton<ICryptographyHelper>(cryptoHelper);
            var authenticateApi = RestService.For<IAuthenticateApi>(internalDataProviderSettings.ServiceUrl);
            services.AddSingleton(authenticateApi);
            var leagueApi = RestService.For<IInternalLeagueApi>(
                new HttpClient(
                    new AuthenticatedHttpClientHandler(authenticateApi.Authenticate, cryptoHelper, appSettings.EncryptKey))
                {
                    BaseAddress = new Uri(internalDataProviderSettings.ServiceUrl.TrimEnd('/'))
                }
                );
            services.AddSingleton(leagueApi);
            services.AddSingleton(RestService.For<ISportRadarLeagueApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<SportRadarLeagueService>();
            services.AddSingleton<SportRadarLeagueService>();
            services.AddSingleton<InternalLeagueService>();
            services.AddSingleton<Func<DataProviderType, ILeagueService>>(serviceProvider => dataProviderType =>
            {
#pragma warning disable S1301 // "switch" statements should have at least 3 "case" clauses
                switch (dataProviderType)
                {
                    case DataProviderType.Internal:
                        return serviceProvider.GetService<InternalLeagueService>();

                    default:
                        return serviceProvider.GetService<SportRadarLeagueService>();
                }
            });
#pragma warning restore S1301 // "switch" statements should have at least 3 "case" clauses

            RegisterLogging(services);
            RegisterRabbitMq(services);
            RegisterSportRadarDataProvider(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
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
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
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

            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);

            var soccerSettings = sportRadarDataProviderSettings.SoccerSettings;
            var healthcheckContainer = new Dictionary<string, DateTime>();
            foreach (var region in soccerSettings.Regions)
            {
                services.AddSingleton<IHostedService>((serviceProvider)
                    => new MatchEventListener(
                        serviceProvider.GetService<IBus>(),
                        new MatchEventListenerService(sportRadarDataProviderSettings, region, serviceProvider.GetService<ILogger>(), healthcheckContainer),
                        serviceProvider.GetService<ILogger>(),
                        serviceProvider.GetService<Func<DataProviderType, ILeagueService>>()));

                healthcheckContainer.Add(region.Name, DateTime.Now);
            }

            services.AddSingleton(healthcheckContainer);
        }
    }
}