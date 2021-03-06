using System;
using Fanex.Caching;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Score247.Shared;
using Score247.Shared.Middlewares;
using Soccer.Cache.Leagues;
using Soccer.Core._Shared.Resources;
using Soccer.EventProcessors.Leagues.Services;
using Soccer.EventProcessors.Matches.Filters;
using Soccer.EventProcessors.Notifications;
using Soccer.EventProcessors.Shared.Middlewares;

namespace Soccer.EventProcessors
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(Configuration);
            services.AddSettings(Configuration);
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<Func<DateTime>>(() => DateTime.Now);
            services.AddRabbitMq(Configuration);
            services.AddHealthCheck();
            services.AddDatabase();

            RegisterFilters(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseHealthCheck();
            app.UseRabbitMq(applicationLifetime);
            app.ConfigureExceptionHandler();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors(options => options.AllowAnyOrigin());

            app.UseHealthCheck();
            app.UseDatabase(Configuration);
        }

        private static void RegisterFilters(IServiceCollection services)
        {
            services.AddSingleton<ILeagueService, LeagueService>();
            services.AddSingleton<ILeagueCache, LeagueCache>();
            services.AddSingleton<ILiveMatchFilter, LiveMatchFilter>();
            services.AddSingleton<ILanguageResourcesService, LanguageResourcesService>();
            services.AddSingleton<IMatchNotificationDeduper, MatchNotificationDeduper>();
        }
    }
}