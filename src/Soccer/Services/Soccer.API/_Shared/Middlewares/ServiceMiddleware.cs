namespace Soccer.API._Shared.Middlewares
{
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.API.Matches;

    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IMatchQueryService, MatchQueryService>();
        }
    }
}