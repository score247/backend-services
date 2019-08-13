namespace Soccer.DataReceivers.ScheduleTasks
{
    using System;
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
    using Soccer.Core.Shared.Configurations;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.Odds;
    using Soccer.DataProviders.SportRadar.Matches.Services;
    using Soccer.DataProviders.SportRadar.Odds;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataReceivers.ScheduleTasks.Matches;
    using Soccer.DataReceivers.ScheduleTasks.Odds;
    using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

    public class Startup
    {
        private const int NumOfWorkers = 2;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            };

            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);

            RegisterLogging(services);
            RegisterRabbitMq(services);
            RegisterSportRadarDataProvider(services);
            RegisterHangfire(services);
            RegisterServices(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);
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

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>(),
                IgnoreAntiforgeryToken = true
            }).UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = NumOfWorkers
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
            var messageQueueSettings = new MessageQueueSettings();
            Configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(x =>
            {
                x.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(
                  cfg =>
                  {
                      cfg.Host(
                           messageQueueSettings.Host,
                           messageQueueSettings.VirtualHost, h =>
                           {
                               h.Username(messageQueueSettings.Username);
                               h.Password(messageQueueSettings.Password);
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

            services.AddSingleton(RestService.For<IOddsApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IOddsService, OddsService>();

            services.AddSingleton(RestService.For<ITimelineApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<ITimelineService, TimelineService>();
        }

        private void RegisterHangfire(IServiceCollection services)
        {
            services.AddHangfire(x
                => x.UseStorage(new MySqlStorage(Configuration.GetConnectionString("Hangfire"))));

            services.AddScoped<IFetchPreMatchesTask, FetchPreMatchesTask>();
            services.AddScoped<IFetchPostMatchesTask, FetchPostMatchesTask>();
            services.AddScoped<IFetchLiveMatchesTask, FetchLiveMatchesTask>();
            services.AddScoped<IFetchOddsScheduleTask, FetchOddsScheduleTask>();
            services.AddScoped<IFetchTimelineTask, FetchTimelineTask>();
        }

        private static void RunHangfireJobs(IAppSettings appSettings)
        {
            var taskSettings = appSettings.ScheduleTasksSettings;

            RecurringJob.AddOrUpdate<IFetchPreMatchesTask>(
                "FetchPreMatch", job => job.FetchPreMatches(taskSettings.FetchMatchScheduleDateSpan), " 0 0/6 * * *");

            RecurringJob.AddOrUpdate<IFetchPostMatchesTask>(
                "FetchPostMatch", job => job.FetchPostMatches(taskSettings.FetchMatchScheduleDateSpan), " 0 0/6 * * *");

            RecurringJob.AddOrUpdate<IFetchLiveMatchesTask>(
                "FetchLiveMatch", job => job.FetchLiveMatches(), "*/45 * * * *");

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchOddsScheduleJobCron))
            {
                RecurringJob.AddOrUpdate<IFetchOddsScheduleTask>(
                    nameof(IFetchOddsScheduleTask.FetchOdds),
                    job => job.FetchOdds(),
                    taskSettings.FetchOddsScheduleJobCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchOddsChangeJobCron))
            {
                RecurringJob.AddOrUpdate<IFetchOddsScheduleTask>(
                nameof(IFetchOddsScheduleTask.FetchOddsChangeLogs),
                job => job.FetchOddsChangeLogs(),
                taskSettings.FetchOddsChangeJobCron);
            }
        }
    }
}