using System;
using System.IO;
using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Score247.Shared;
using Soccer.API.AccountSettings;
using Soccer.API.Favorites;
using Soccer.API.Leagues;
using Soccer.API.Matches;
using Soccer.API.Matches.Helpers;
using Soccer.API.News;
using Soccer.API.Odds;
using Soccer.API.Teams;
using Soccer.Cache.Leagues;
using Soccer.Core._Shared.Helpers;
using Svg;

namespace Soccer.API.Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<ICryptographyHelper, CryptographyHelper>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<ILeagueCache, LeagueCache>();

            services.AddScoped<IMatchQueryService, MatchQueryService>();
            services.AddScoped<IOddsQueryService, OddsQueryService>();
            services.AddScoped<ILeagueQueryService, LeagueQueryService>();
            services.AddScoped<ILeagueCommandService, LeagueCommandService>();
            services.AddScoped<ITeamQueryService, TeamQueryService>();

            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);
            services.AddSingleton<IMatchLineupsGenerator>(new MatchLineupsSvgGenerator(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"_Shared/App_Data/SvgFiles"),
                SvgDocument.Open
                ));

            services.AddScoped<INewsQueryService, NewsQueryService>();
            services.AddScoped<IFavoriteCommandService, FavoriteCommandService>();
            services.AddScoped<IFavoriteQueryService, FavoriteQueryService>();
            services.AddScoped<IAccountSettingCommandService, AccountSettingCommandService>();
        }
    }
}