namespace Soccer.DataReceivers.ScheduleTasks
{
    using System;
    using System.Linq;
    using Hangfire;
    using Hangfire.Dashboard;
    using Hangfire.PostgreSql;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Refit;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar._Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Matches.Services;
    using Soccer.DataReceivers.ScheduleTasks._Shared.Configurations;
    using Soccer.DataReceivers.ScheduleTasks.Match;

    public class Startup
    {
        private const int OneYearDays = 365;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings(Configuration);
            services.AddSingleton<IAppSettings>(appSettings);

            RegisterSportRadarDataProvider(services);
            RegisterHangfireJobs(services);

            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"),
                   new PostgreSqlStorageOptions
                   {
                       InvisibilityTimeout = TimeSpan.FromDays(OneYearDays)
                   }));

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

        private void RegisterSportRadarDataProvider(IServiceCollection services)
        {
            var sportRadarDataProviderSettings = new SportRadarSettings();
            Configuration.GetSection("DataProviders:SportRadar").Bind(sportRadarDataProviderSettings);

            services.AddSingleton<ISportRadarSettings>(sportRadarDataProviderSettings);
            services.AddSingleton(RestService.For<IMatchApi>(sportRadarDataProviderSettings.ServiceUrl));
            services.AddSingleton<IMatchService, MatchService>();
        }

        private void RegisterHangfireJobs(IServiceCollection services)
        {
            services.AddScoped<IFetchMatchScheduleTask, FetchMatchScheduleTask>();
        }

        private static void RunHangfireJobs(IAppSettings appSettings)
        {
            RecurringJob.AddOrUpdate<IFetchMatchScheduleTask>(
                "FetchMatchSchedule", job => job.FetchSchedule(appSettings.ScheduleTasksSettings.FetchMatchScheduleDateSpan), " 0 0/6 * * *");
        }
    }
}