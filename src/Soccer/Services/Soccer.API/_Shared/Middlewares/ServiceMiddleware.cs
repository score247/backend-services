using Soccer.API.Leagues;

namespace Soccer.API.Shared.Middlewares
{
    using System;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using Score247.Shared;
    using Soccer.API.Matches;
    using Soccer.API.Odds;

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
        }
    }
}