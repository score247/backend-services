using System.Collections.Generic;
using Soccer.Core.Matches.Models;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

    public interface IFetchPostMatchesTask
    {
        [Queue("medium")]
        void FetchPostMatches(int dateSpan);

        [Queue("medium")]
        Task FetchPostMatchesForDate(DateTime date, Language language);
    }

    public class FetchPostMatchesTask : IFetchPostMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;
        private readonly IFetchMatchLineupsTask fetchMatchLineupsTask;

        public FetchPostMatchesTask(
            IBus messageBus, 
            IAppSettings appSettings, 
            IMatchService matchService,
            IFetchMatchLineupsTask fetchMatchLineupsTask)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
            this.fetchMatchLineupsTask = fetchMatchLineupsTask;
        }

        public void FetchPostMatches(int dateSpan)
        {
            var from = DateTime.UtcNow.AddDays(-dateSpan);
            var to = DateTime.UtcNow;

            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var date = from; date.Date <= to; date = date.AddDays(1))
                {
                    BackgroundJob.Enqueue<IFetchPostMatchesTask>(t => t.FetchPostMatchesForDate(date, language));
                }
            }
        }

        public async Task FetchPostMatchesForDate(DateTime date, Language language)
        {
            int batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var matches = await matchService.GetPostMatches(date, language);

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var matchesBatch = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPostMatchFetchedMessage>(new PostMatchFetchedMessage(matchesBatch, language.DisplayName));
            }

            FetchTeamHeadToHead(language, matches);
            
            foreach (var match in matches)
            {
                await fetchMatchLineupsTask.FetchMatchLineups(match.Id, match.Region);
            }
        }
   
        private static void FetchTeamHeadToHead(Language language, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                var homeTeamId = match.Teams.FirstOrDefault(t => t.IsHome)?.Id;
                var awayTeamId = match.Teams.FirstOrDefault(t => !t.IsHome)?.Id;

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchHeadToHeads(homeTeamId, awayTeamId, language));
            }
        }
    }
}