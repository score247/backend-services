using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Soccer.DataReceivers.ScheduleTasks.News;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(
                globalConfiguration
                =>
                {
                    globalConfiguration
                        .UseStorage(new MySqlStorage(configuration.GetConnectionString("Hangfire")));
                });

            services.AddScoped<IFetchPreMatchesTask, FetchPreMatchesTask>();
            services.AddScoped<IFetchPostMatchesTask, FetchPostMatchesTask>();
            services.AddScoped<IFetchLiveMatchesTask, FetchLiveMatchesTask>();
            services.AddScoped<IFetchTimelineTask, FetchTimelineTask>();
            services.AddScoped<IFetchLiveMatchesTimelineTask, FetchLiveMatchesTimelineTask>();
            services.AddScoped<IFetchPreMatchesTimelineTask, FetchPreMatchesTimelineTask>();
            services.AddScoped<IFetchLeaguesTask, FetchLeaguesTask>();
            services.AddScoped<IFetchHeadToHeadsTask, FetchHeadToHeadsTask>();
            services.AddScoped<IFetchMatchLineupsTask, FetchMatchLineupsTask>();
            services.AddScoped<ICleanMajorLeaguesCacheTask, CleanMajorLeaguesCacheTask>();
            services.AddScoped<IFetchLeaguesSeasonTask, FetchLeaguesSeasonTask>();
            services.AddScoped<IFetchLeagueMatchesTask, FetchLeagueMatchesTask>();
            services.AddScoped<IFetchLeagueStandingsTask, FetchLeagueStandingsTask>();
            services.AddScoped<IFetchLeagueHeadToHeadTask, FetchLeagueHeadToHeadTask>();
        }

#pragma warning disable S138 // Functions should not have too many lines of code
#pragma warning disable S1541 // Methods and properties should not be too complex

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning restore S138 // Functions should not have too many lines of code
        {
            var appSettings = app.ApplicationServices.GetService<IAppSettings>();
            var taskSettings = appSettings.ScheduleTasksSettings;

            if (appSettings.EnabledHangfireUI)
            {
                app.UseHangfireDashboard(options: new DashboardOptions
                {
                    Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>(),
                    IgnoreAntiforgeryToken = true
                });
            }

            app.UseHangfireServer(options: new BackgroundJobServerOptions
            {
                ServerName = $"{configuration.GetSection("HangfireServerName").Get<string>()}-{Guid.NewGuid().ToString()}",
                WorkerCount = configuration.GetSection("HangfireWorkers").Get<int>(),
                Queues = configuration.GetSection("HangfireQueues").Get<string[]>()
            });

            RegisterTask<IFetchPreMatchesTask>(taskSettings.FetchPreMatchesCron, job => job.FetchPreMatches());
            RegisterTask<IFetchPostMatchesTask>(taskSettings.FetchPostMatchesCron, job => job.FetchPostMatches());
            RegisterTask<IFetchLiveMatchesTask>(taskSettings.FetchLiveMatchesCron, job => job.FetchLiveMatches());
            RegisterTask<IFetchLiveMatchesTimelineTask>(taskSettings.FetchLiveMatchesTimelineCron, job => job.FetchLiveMatchesTimeline());
            RegisterTask<IFetchLeaguesTask>(taskSettings.FetchLeaguesCron, job => job.FetchLeagues());
            RegisterTask<IFetchMatchLineupsTask>(taskSettings.FetchMatchLineupsCron, job => job.FetchMatchLineups());
            RegisterTask<ICleanMajorLeaguesCacheTask>(taskSettings.CleanMajorLeaguesCacheCron, job => job.CleanMajorLeaguesCache());
            RegisterTask<IFetchLeaguesSeasonTask>(taskSettings.FetchLeaguesSeasonCron, job => job.FetchLeaguesSeason());
            RegisterTask<IFetchLeagueMatchesTask>(taskSettings.FetchLeagueMatchesAndTimelinesCron, job => job.FetchLeagueMatchesAndTimelineEvents());
            RegisterTask<IFetchLeagueStandingsTask>(taskSettings.FetchLeagueStandingCron, job => job.FetchLeagueStandings());
            RegisterTask<IFetchLeagueMatchesTask>(taskSettings.FetchLeagueMatchesCron, job => job.FetchLeagueMatches());
            RegisterTask<IFetchNewsTask>(taskSettings.FetchNewsFeedCron, job => job.FetchNewsFeed());
            RegisterTask<IFetchLeagueMatchesTask>(taskSettings.FetchTeamResultsForMajorLeaguesCron, job => job.FetchTeamResultsForMajorLeagues());
            RegisterTask<IFetchNewsTask>(taskSettings.FetchNewsFeedCron, job => job.FetchNewsFeed());
        }

        private static void RegisterTask<T>(string cronExpression, Expression<Func<T, Task>> methodCall)
        {
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                var info = (MethodCallExpression)methodCall.Body;

                RecurringJob.AddOrUpdate(
                            info.Method.Name,
                            methodCall,
                            cronExpression);
            }
        }
    }
}