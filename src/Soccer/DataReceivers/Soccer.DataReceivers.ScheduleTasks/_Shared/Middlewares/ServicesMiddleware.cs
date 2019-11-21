using System;
using Fanex.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Score247.Shared;
using Soccer.Cache.Leagues;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Internal._Share.Configurations;
using Soccer.DataProviders.Internal.Leagues.Services;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataProviders.Odds;
using Soccer.DataProviders.SportRadar.Leagues.Services;
using Soccer.DataProviders.SportRadar.Matches.Services;
using Soccer.DataProviders.SportRadar.Odds;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Teams.Services;
using Soccer.DataProviders.Teams.Services;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares
{
    public static class ServicesMiddleware
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);

            var sportRadarDataProviderSettings = new SportRadarSettings();
            configuration.GetSection("DataProviders:SportRadar").Bind(sportRadarDataProviderSettings);

            var internalDataProviderSettings = new InternalProviderSettings();
            configuration.GetSection("DataProviders:Internal").Bind(internalDataProviderSettings);

            services.AddSingleton<ISportRadarSettings>(sportRadarDataProviderSettings);

            services.AddSingleton(RestService.For<IMatchApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IMatchService, MatchService>();

            services.AddSingleton(RestService.For<IOddsApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IOddsService, OddsService>();

            services.AddSingleton(RestService.For<ITimelineApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<ITimelineService, TimelineService>();

            services.AddSingleton<ILeagueCache, LeagueCache>();
            services.AddSingleton(RestService.For<ISportRadarLeagueApi>(sportRadarDataProviderSettings.DefaultServiceUrl));
            services.AddSingleton(RestService.For<IInternalLeagueApi>(internalDataProviderSettings.ServiceUrl));
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
        }
    }
}