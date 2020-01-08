using System;
using System.Net.Http;
using Fanex.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Score247.Shared;
using Soccer.Cache.Leagues;
using Soccer.Core._Shared.Helpers;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.EyeFootball._Shared.Configurations;
using Soccer.DataProviders.EyeFootball.News.Services;
using Soccer.DataProviders.Internal._Share.Configurations;
using Soccer.DataProviders.Internal.Leagues.Services;
using Soccer.DataProviders.Internal.Share.Helpers;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataProviders.News.Services;
using Soccer.DataProviders.Odds;
using Soccer.DataProviders.SportRadar.Leagues.Services;
using Soccer.DataProviders.SportRadar.Matches.Services;
using Soccer.DataProviders.SportRadar.Odds;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Teams.Services;
using Soccer.DataProviders.Teams.Services;
using Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares
{
    public static class ServicesMiddleware
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration, IAppSettings appSettings)
        {
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);

            var sportRadarDataProviderSettings = new SportRadarSettings();
            configuration.GetSection("DataProviders:SportRadar").Bind(sportRadarDataProviderSettings);

            var internalDataProviderSettings = new InternalProviderSettings();
            configuration.GetSection("DataProviders:Internal").Bind(internalDataProviderSettings);

            var eyeFootballDataProviderSettings = new EyeFootballSettings();
            configuration.GetSection("DataProviders:EyeFootball").Bind(eyeFootballDataProviderSettings);

            services.AddSingleton<ISportRadarSettings>(sportRadarDataProviderSettings);
            services.AddSingleton<IEyeFootballSettings>(eyeFootballDataProviderSettings);

            services.AddSingleton(RestService.For<IMatchApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IMatchService, MatchService>();

            services.AddSingleton(RestService.For<IOddsApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IOddsService, OddsService>();

            services.AddSingleton(RestService.For<ITimelineApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<ITimelineService, TimelineService>();

            services.AddSingleton<ILeagueCache, LeagueCache>();
            services.AddSingleton(RestService.For<ISportRadarLeagueApi>(sportRadarDataProviderSettings.ServiceUrl));
            var authenticateApi = RestService.For<IAuthenticateApi>(internalDataProviderSettings.ServiceUrl);
            services.AddSingleton(authenticateApi);
            var cryptoHelper = new CryptographyHelper();
            services.AddSingleton<ICryptographyHelper>(cryptoHelper);
            var leagueApi = RestService.For<IInternalLeagueApi>(
                new HttpClient(
                    new AuthenticatedHttpClientHandler(authenticateApi.Authenticate, cryptoHelper, appSettings.EncryptKey))
                {
                    BaseAddress = new Uri(internalDataProviderSettings.ServiceUrl.TrimEnd('/'))
                }
                );
            services.AddSingleton(leagueApi);
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

            services.AddSingleton<ILeagueScheduleService, SportRadarLeagueService>();
            services.AddSingleton<ILeagueSeasonService, InternalLeagueService>();

            services.AddSingleton(RestService.For<IHeadToHeadApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IHeadToHeadService, HeadToHeadService>();

            services.AddSingleton<IHealthCheckService, HealthCheckService>();
            services.AddSingleton<INewsService, NewsService>();
        }
    }
}