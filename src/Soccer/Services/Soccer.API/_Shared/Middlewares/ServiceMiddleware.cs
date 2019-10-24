using Soccer.API.Leagues;

namespace Soccer.API.Shared.Middlewares
{
    using System;
    using System.IO;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using Score247.Shared;
    using Soccer.API.Matches;
    using Soccer.API.Matches.Helpers;
    using Soccer.API.Odds;
    using Svg;

    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IMatchQueryService, MatchQueryService>();
            services.AddScoped<IOddsQueryService, OddsQueryService>();
            services.AddScoped<ILeagueQueryService, LeagueQueryService>();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);
            services.AddSingleton<IMatchLineupsGenerator>(new MatchLineupsSvgGenerator(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"_Shared\App_Data\SvgFiles"),
                (path) => SvgDocument.Open(path)
                ));
        }
    }
}