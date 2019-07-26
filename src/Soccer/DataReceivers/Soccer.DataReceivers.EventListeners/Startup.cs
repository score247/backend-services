﻿namespace Soccer.DataReceivers.EventListeners
{
    using System.Linq;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Hangfire;
    using Hangfire.Dashboard;
    using Hangfire.MySql.Core;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Refit;
    using Sentry;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar._Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Matches.Services;
    using Soccer.DataReceivers.EventListeners._Shared.Configurations;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);
            RegisterLogging(services);
            RegisterRabbitMq(services);
            RegisterSportRadarDataProvider(services);
            RegisterHangfire(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var appSettings = app.ApplicationServices.GetService<IAppSettings>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>()
            }).UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = 2
            });

            RunHangfireJobs(appSettings);

            app.UseStaticFiles();
            app.UseMvc();
        }

        public void RegisterLogging(IServiceCollection services)
        {
            LogManager
                   .SetDefaultLogCategory(Configuration["Fanex.Logging:DefaultCategory"])
                   .Use(new SentryLogging(new SentryEngineOptions
                   {
                       Dsn = new Dsn(Configuration["Fanex.Logging:SentryUrl"])
                   }));

            services.AddSingleton(Logger.Log);
        }

        private void RegisterRabbitMq(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(
                  cfg =>
                  {
                      cfg.Host(Configuration.GetSection("MessageQueue:RabbitMQ:Host").Value, "/", h =>
                      {
                          h.Username(Configuration.GetSection("MessageQueue:RabbitMQ:UserName").Value);
                          h.Password(Configuration.GetSection("MessageQueue:RabbitMQ:Password").Value);
                      });
                  }));
            });
        }

        private void RegisterSportRadarDataProvider(IServiceCollection services)
        {
            var sportRadarDataProviderSettings = new SportRadarSettings();
            Configuration.GetSection("DataProviders:SportRadar").Bind(sportRadarDataProviderSettings);

            services.AddSingleton<ISportRadarSettings>(sportRadarDataProviderSettings);
            services.AddSingleton(RestService.For<IMatchApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IMatchService, MatchService>();
        }

        private void RegisterHangfire(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(Configuration.GetConnectionString("Hangfire"))));
        }

        private static void RunHangfireJobs(IAppSettings appSettings)
        {
        }
    }
}