using System;
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
            var from = DateTime.UtcNow;
            var to = DateTime.UtcNow.AddDays(dateSpan);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var date = from; date.Date <= to; date = date.AddDays(1))
                {
                    BackgroundJob.Enqueue<IFetchPreMatchesTask>(t => t.FetchPreMatchesForDate(date, language));
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
            FetchTeamHeadToHeads(language, matches);
            FetchTeamResults(language, matches);
        }

        private async Task PublishPreMatchFetchedMessage(Language language, int batchSize, IList<Match> matches)
        {
            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var matchesBatch = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPreMatchesFetchedMessage>(
                    new PreMatchesFetchedMessage(matchesBatch, language.DisplayName));

                BackgroundJob.Enqueue<IFetchPreMatchesTimelineTask>(t => t.FetchPreMatchTimeline(matchesBatch.ToList()));
            }
        }

        private static void FetchTeamHeadToHeads(Language language, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                var homeTeamId = match.Teams.FirstOrDefault(t => t.IsHome)?.Id;
                var awayTeamId = match.Teams.FirstOrDefault(t => !t.IsHome)?.Id;

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchHeadToHeads(homeTeamId, awayTeamId, language));
            }
        }

        private static void FetchTeamResults(Language language, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                var homeTeamId = match.Teams.FirstOrDefault(t => t.IsHome)?.Id;
                var awayTeamId = match.Teams.FirstOrDefault(t => !t.IsHome)?.Id;

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchTeamResults(homeTeamId, language));

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchTeamResults(awayTeamId, language));
            }
        }
    }
}