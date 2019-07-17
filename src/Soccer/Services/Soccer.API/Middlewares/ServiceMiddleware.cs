namespace Soccer.API.Middlewares
{
    using Fanex.Caching;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.API.Modules.Matches;
    using Soccer.Core.Domain.Matches;

    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();

            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<ILiveMatchRepository, LiveMatchRepository>();
            services.AddScoped<IMatchQueryService, MatchQueryService>();
        }
    }
}