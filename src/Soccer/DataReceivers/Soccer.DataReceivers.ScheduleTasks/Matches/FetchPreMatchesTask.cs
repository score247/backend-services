﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchPreMatchesTask
    {
        [Queue("low")]
        void FetchPreMatches(int dateSpan);

        [Queue("low")]
        Task FetchPreMatchesForDate(DateTime date, Language language);
    }

    public class FetchPreMatchesTask : IFetchPreMatchesTask
    {
        private readonly IAppSettings appSettings;
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchPreMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            IMatchService matchService)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public void FetchPreMatches(int dateSpan)
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var dayAdd = 0; dayAdd <= dateSpan; dayAdd++)
                {
                    var fetchDate = DateTime.UtcNow.AddDays(dayAdd);

                    if (dayAdd > 0)
                    {
                        BackgroundJob.Schedule<IFetchPreMatchesTask>(
                            t => t.FetchPreMatchesForDate(fetchDate, language),
                            TimeSpan.FromHours(appSettings.ScheduleTasksSettings.FetchMatchesByDateDelayedHours * dayAdd));
                    }
                    else
                    {
                        BackgroundJob.Enqueue<IFetchPreMatchesTask>(t => t.FetchPreMatchesForDate(fetchDate, language));
                    }
                }
            }
        }

        public async Task FetchPreMatchesForDate(DateTime date, Language language)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;

            var matches = (await matchService.GetPreMatches(date, language))
                                .Where(x => x.MatchResult.EventStatus != MatchStatus.Live &&
                                            x.MatchResult.EventStatus != MatchStatus.Closed).ToList();

            await PublishPreMatchFetchedMessage(language, batchSize, matches);
        }

        private async Task PublishPreMatchFetchedMessage(Language language, int batchSize, ICollection<Match> matches)
        {
            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var batchOfMatches = matches.Skip(i * batchSize).Take(batchSize).ToList();

                await messageBus.Publish<IPreMatchesFetchedMessage>(
                    new PreMatchesFetchedMessage(batchOfMatches, language.DisplayName));

                BackgroundJob.Enqueue<IFetchPreMatchesTimelineTask>(
                    task => task.FetchPreMatchTimeline(batchOfMatches));

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(
                    task => task.FetchHeadToHeads(language, batchOfMatches));

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(
                    task => task.FetchTeamResults(language, batchOfMatches));
            }
        }
    }
}