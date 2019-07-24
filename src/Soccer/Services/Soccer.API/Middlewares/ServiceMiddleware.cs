namespace Soccer.API.Middlewares
{
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.API.Modules.Matches;
    using Soccer.Core.Domain.Matches.Repositories;

    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IMatchQueryService, MatchQueryService>();
        }
    }
}