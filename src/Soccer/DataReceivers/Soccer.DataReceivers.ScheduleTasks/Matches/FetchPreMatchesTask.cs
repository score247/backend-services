using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchPreMatchesTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchPreMatches();

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchPreMatchesForDate(DateTime date, Language language, IEnumerable<League> majorLeagues);
    }

    public class FetchPreMatchesTask : IFetchPreMatchesTask
    {
        private readonly IAppSettings appSettings;
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly ILeagueService internalLeagueService;
        private readonly IBackgroundJobClient jobClient;

        public FetchPreMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            IMatchService matchService,
            Func<DataProviderType, ILeagueService> leagueServiceFactory,
            IBackgroundJobClient jobClient)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
            this.jobClient = jobClient;
        }

        public async Task FetchPreMatches()
        {
            var majorLeagues = (await internalLeagueService.GetLeagues(Language.en_US))?.ToList();

            if (majorLeagues?.Any() != true)
            {
                return;
            }

            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var dayAdd = 0; dayAdd < appSettings.ScheduleTasksSettings.FetchMatchScheduleDateSpan; dayAdd++)
                {
                    var fetchDate = DateTime.UtcNow.AddDays(dayAdd);
                    var delayedHour = appSettings.ScheduleTasksSettings.FetchMatchesByDateDelayedHours + dayAdd - 1;

                    jobClient.Schedule<IFetchPreMatchesTask>(
                        t => t.FetchPreMatchesForDate(fetchDate, language, majorLeagues),
                        TimeSpan.FromHours(delayedHour));
                }
            }
        }

        public async Task FetchPreMatchesForDate(DateTime date, Language language, IEnumerable<League> majorLeagues)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;

            var matches = (await matchService.GetPreMatches(date, language))
                .Where(match =>
                    !match.MatchResult.EventStatus.IsLive() &&
                    !match.MatchResult.EventStatus.IsClosed() &&
                    majorLeagues?.Any(league => league.Id == match.League.Id) == true)
                .ToList();

            await PublishPreMatchFetchedMessage(language, batchSize, matches);
            await PublishLeagueGroupFetchedMessage(language, matches);

            FetchPreMatchLeagueStanding(language, matches);
        }

        private async Task PublishLeagueGroupFetchedMessage(Language language, List<Match> matches)
        {
            var matchGroupByStage = matches
                            .GroupBy(match =>
                                (match.League.Id, match.LeagueGroupName, match.LeagueSeason, match.LeagueRound));

            if (matchGroupByStage == null)
            {
                return;
            }

            foreach (var groupStage in matchGroupByStage)
            {
                await messageBus.Publish<ILeagueGroupFetchedMessage>(
                    new LeagueGroupFetchedMessage(
                        groupStage.Key.Id,
                        groupStage.Key.LeagueSeason.Id,
                        groupStage.Key.LeagueGroupName,
                        groupStage.Key.LeagueRound,
                        language));
            }
        }

        private async Task PublishPreMatchFetchedMessage(Language language, int batchSize, ICollection<Match> matches)
        {
            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var batchOfMatches = matches.Skip(i * batchSize).Take(batchSize).ToList();

                await messageBus.Publish<IPreMatchesFetchedMessage>(
                    new PreMatchesFetchedMessage(batchOfMatches, language.DisplayName));

                jobClient.Enqueue<IFetchPreMatchesTimelineTask>(
                    task => task.FetchPreMatchTimeline(batchOfMatches));
            }
        }

        private void FetchPreMatchLeagueStanding(Language language, ICollection<Match> matches)
        {
            var leagueWithMatchesGroups = matches.GroupBy(match => new { match.League.Id, match.League.Region });

            foreach (var leagueWithMatches in leagueWithMatchesGroups)
            {
                jobClient.Enqueue<IFetchLeagueStandingsTask>(task => task.FetchLeagueStandings(
                    leagueWithMatches.Key.Id,
                    leagueWithMatches.Key.Region,
                    language,
                    false));
            }
        }
    }
}