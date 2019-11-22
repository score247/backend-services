﻿using System.Linq;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Soccer.DataReceivers.ScheduleTasks.Odds;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(x
                => x.UseStorage(new MySqlStorage(configuration.GetConnectionString("Hangfire"))));

            services.AddScoped<IFetchPreMatchesTask, FetchPreMatchesTask>();
            services.AddScoped<IFetchPostMatchesTask, FetchPostMatchesTask>();
            services.AddScoped<IFetchLiveMatchesTask, FetchLiveMatchesTask>();
            services.AddScoped<IFetchOddsScheduleTask, FetchOddsScheduleTask>();
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
        }

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            var appSettings = app.ApplicationServices.GetService<IAppSettings>();
            var taskSettings = appSettings.ScheduleTasksSettings;

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>(),
                IgnoreAntiforgeryToken = true
            }).UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = configuration.GetSection("HangfireWorkers").Get<int>(),
                Queues = configuration.GetSection("HangfireQueues").Get<string[]>()
            });

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchPreMatchesCron))
            {
                RecurringJob.AddOrUpdate<IFetchPreMatchesTask>(
                    nameof(IFetchPreMatchesTask.FetchPreMatches),
                    job => job.FetchPreMatches(taskSettings.FetchMatchScheduleDateSpan),
                    taskSettings.FetchPreMatchesCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchPostMatchesCron))
            {
                RecurringJob.AddOrUpdate<IFetchPostMatchesTask>(
                    nameof(IFetchPostMatchesTask.FetchPostMatches),
                    job => job.FetchPostMatches(taskSettings.FetchMatchScheduleDateSpan),
                    taskSettings.FetchPostMatchesCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchLiveMatchesCron))
            {
                RecurringJob.AddOrUpdate<IFetchLiveMatchesTask>(
                    nameof(IFetchLiveMatchesTask.FetchLiveMatches),
                    job => job.FetchLiveMatches(),
                    taskSettings.FetchLiveMatchesCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchLiveMatchesTimelineCron))
            {
                RecurringJob.AddOrUpdate<IFetchLiveMatchesTimelineTask>(
                    nameof(IFetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline),
                    job => job.FetchLiveMatchesTimeline(),
                    taskSettings.FetchLiveMatchesTimelineCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchLeaguesCron))
            {
                RecurringJob.AddOrUpdate<IFetchLeaguesTask>(
                nameof(IFetchLeaguesTask.FetchLeagues),
                job => job.FetchLeagues(),
                taskSettings.FetchLeaguesCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchMatchLineupsCron))
            {
                RecurringJob.AddOrUpdate<IFetchMatchLineupsTask>(
                nameof(IFetchMatchLineupsTask.FetchMatchLineups),
                job => job.FetchMatchLineups(),
                taskSettings.FetchMatchLineupsCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.CleanMajorLeaguesCacheCron))
            {
                RecurringJob.AddOrUpdate<ICleanMajorLeaguesCacheTask>(
                nameof(ICleanMajorLeaguesCacheTask.CleanMajorLeaguesCache),
                job => job.CleanMajorLeaguesCache(),
                taskSettings.CleanMajorLeaguesCacheCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchLeaguesSeasonCron))
            {
                RecurringJob.AddOrUpdate<IFetchLeaguesSeasonTask>(
                nameof(IFetchLeaguesSeasonTask.FetchLeaguesSeason),
                job => job.FetchLeaguesSeason(),
                taskSettings.FetchLeaguesSeasonCron);
            }

            if (!string.IsNullOrWhiteSpace(taskSettings.FetchLeagueMatchesCron))
            {
                RecurringJob.AddOrUpdate<IFetchLeagueMatchesTask>(
                nameof(IFetchLeagueMatchesTask.FetchLeagueMatches),
                job => job.FetchLeagueMatches(),
                taskSettings.FetchLeaguesSeasonCron);
            }
        }
    }
}