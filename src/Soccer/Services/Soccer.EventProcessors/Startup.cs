namespace Soccer.EventProcessors
{
    using System;
    using System.Collections.Generic;
    using Fanex.Caching;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.Core.Matches.Models;
    using Soccer.EventProcessors._Shared.Filters;
    using Soccer.EventProcessors.Leagues;
    using Soccer.EventProcessors.Shared.Middlewares;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(Configuration);
            services.AddSettings(Configuration);
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<Func<DateTime>>(() => DateTime.Now);
            services.AddRabbitMq(Configuration);
            services.AddHealthCheck();
            services.AddDatabase();

            RegisterFilters(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseHealthCheck();
            app.UseRabbitMq(applicationLifetime);
            app.ConfigureExceptionHandler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

            app.UseCors(options => options.AllowAnyOrigin());

            app.UseHealthCheck();
            app.UseDatabase(Configuration);
        }

        private static void RegisterFilters(IServiceCollection services)
        {
            services.AddSingleton<IFilter<IEnumerable<Match>, IEnumerable<Match>>, LeagueFilter>();
            services.AddSingleton<IFilter<Match, bool>, LeagueFilter>();
            services.AddSingleton<IFilter<MatchEvent, bool>, LeagueFilter>();
        }
    }
}