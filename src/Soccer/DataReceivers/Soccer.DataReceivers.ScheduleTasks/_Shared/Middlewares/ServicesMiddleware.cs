﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataProviders.Odds;
using Soccer.DataProviders.SportRadar.Matches.Services;
using Soccer.DataProviders.SportRadar.Odds;
using Soccer.DataProviders.SportRadar.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares
{
    public static class ServicesMiddleware
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);

            var sportRadarDataProviderSettings = new SportRadarSettings();
            configuration.GetSection("DataProviders:SportRadar").Bind(sportRadarDataProviderSettings);

            services.AddSingleton<ISportRadarSettings>(sportRadarDataProviderSettings);

            services.AddSingleton(RestService.For<IMatchApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IMatchService, MatchService>();

            services.AddSingleton(RestService.For<IOddsApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IOddsService, OddsService>();

            services.AddSingleton(RestService.For<ITimelineApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<ITimelineService, TimelineService>();
        }
    }
}