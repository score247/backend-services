using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeagueMatchesTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchLeagueMatches();

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchLeagueMatchesAndTimelineEvents();

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchMatchesForLeague(IList<LeagueSeasonProcessedInfo> leagueSeasons, bool isScheduleTimelineEvents = true);

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchMatchesForLeague(IList<League> leagues);
    }

    public class FetchLeagueMatchesTask : IFetchLeagueMatchesTask
    {
        private const int BatchOfLeagueSize = 5;
        private const int BatchOfMatchSize = 15;
        private readonly TimeSpan TeamResultsDelayTimespan;

        private readonly IAppSettings appSettings;
        private readonly IBus messageBus;

        private readonly ILeagueScheduleService leagueScheduleService;
        private readonly ILeagueSeasonService leagueSeasonService;
        private readonly ILeagueService leagueService;
        private readonly IBackgroundJobClient jobClient;

        public FetchLeagueMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            ILeagueScheduleService leagueScheduleService,
            ILeagueSeasonService leagueSeasonService,
            IBackgroundJobClient jobClient,
            Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.leagueScheduleService = leagueScheduleService;
            this.leagueSeasonService = leagueSeasonService;
            this.jobClient = jobClient;
            this.leagueService = leagueServiceFactory(DataProviderType.Internal);

            TeamResultsDelayTimespan = TimeSpan.FromMinutes(appSettings.ScheduleTasksSettings.FetchTeamResultsDelayedMinutes);
        }

        public async Task FetchLeagueMatches()
        {
            var leagues = (await leagueService.GetLeagues(Language.en_US))?.ToList();

            if (leagues == null || leagues.Count == 0)
            {
                return;
            }

            for (var i = 0; i * BatchOfLeagueSize < leagues.Count; i++)
            {
                var batchOfLeague = leagues.Skip(i * BatchOfLeagueSize).Take(BatchOfLeagueSize).ToList();

                jobClient.Enqueue<IFetchLeagueMatchesTask>(t => t.FetchMatchesForLeague(batchOfLeague));
            }
        }

        // Should only execute when enable new major league since it will fetch lots of data for post matches
        public async Task FetchLeagueMatchesAndTimelineEvents()
        {
            var unprocessedLeagueSeason = (await leagueSeasonService.GetUnprocessedLeagueSeason())?.ToList();

            if (unprocessedLeagueSeason == null || unprocessedLeagueSeason.Count == 0)
            {
                return;
            }

            for (var i = 0; i * BatchOfLeagueSize < unprocessedLeagueSeason.Count; i++)
            {
                var batchOfLeague = unprocessedLeagueSeason.Skip(i * BatchOfLeagueSize).Take(BatchOfLeagueSize).ToList();

                jobClient.Enqueue<IFetchLeagueMatchesTask>(t => t.FetchMatchesForLeague(batchOfLeague, true));
            }
        }

        public async Task FetchMatchesForLeague(IList<LeagueSeasonProcessedInfo> leagueSeasons, bool isScheduleTimelineEvents = true)
        {
            foreach (var season in leagueSeasons)
            {
                foreach (var language in Enumeration.GetAll<Language>())
                {
                    var matches = (await leagueScheduleService.GetLeagueMatches(season.Region, season.LeagueId, language)).ToList();

                    await PublishPreMatchesMessage(language, matches);

                    ScheduleTeamResultsTasks(language, matches);

                    if (isScheduleTimelineEvents)
                    {
                        ScheduleTimelineAndLineUpsTasks(matches.Where(match => match.MatchResult.EventStatus.IsClosed()).ToList(), language);
                    }
                }
            }

            await messageBus.Publish<ILeagueMatchesFetchedMessage>(
                new LeagueMatchesFetchedMessage(leagueSeasons));
        }

        public async Task FetchMatchesForLeague(IList<League> leagues)
        {
            foreach (var league in leagues)
            {
                foreach (var language in Enumeration.GetAll<Language>())
                {
                    var matches = (await leagueScheduleService.GetLeagueMatches(league.Region, league.Id, language)).ToList();

                    await PublishPreMatchesMessage(language, matches);
                }
            }
        }

        private async Task PublishPreMatchesMessage(Language language, IList<Match> matches)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var batchOfMatches = matches.Skip(i * batchSize).Take(batchSize).ToList();

                await messageBus.Publish<IPreMatchesFetchedMessage>(
                    new PreMatchesFetchedMessage(batchOfMatches, language.DisplayName));
            }
        }

        private void ScheduleTimelineAndLineUpsTasks(IList<Match> closedMatches, Language language)
        {
            for (var i = 0; i * BatchOfMatchSize < closedMatches.Count; i++)
            {
                var batchOfMatches = closedMatches.Skip(i * BatchOfMatchSize).Take(BatchOfMatchSize).ToList();

                var timelineDelayTimespan = TimeSpan.FromMinutes(appSettings.ScheduleTasksSettings.FetchTimelineDelayedMinutes + i);
                var lineupsDelayTimespan = TimeSpan.FromMinutes(appSettings.ScheduleTasksSettings.FetchLineupsDelayedMinutes + i);

                jobClient.Schedule<IFetchTimelineTask>(t => t.FetchTimelineEventsForClosedMatch(batchOfMatches, language), timelineDelayTimespan);
                jobClient.Schedule<IFetchMatchLineupsTask>(t => t.FetchMatchLineupsForClosedMatch(batchOfMatches, language), lineupsDelayTimespan);
            }
        }

        private void ScheduleTeamResultsTasks(Language language, IEnumerable<Match> matches)
        {
            var teams = matches
                .SelectMany(match => match.Teams)
                .GroupBy(team => team.Id)
                .Select(grp => grp.FirstOrDefault())
                .ToList();

            if (teams.Count > 0)
            {
                jobClient.Schedule<IFetchHeadToHeadsTask>(task => task.FetchTeamResults(teams, language), TeamResultsDelayTimespan);
            }
        }
    }
}