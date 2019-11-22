using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeagueStandingsTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task FetchLeagueStandings(string leagueId, string region, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        void FetchClosedMatchesStanding(IEnumerable<Match> closedMatches, Language language);
    }

    public class FetchLeagueStandingsTask : IFetchLeagueStandingsTask
    {
        private readonly IBus messageBus;
        private readonly ILeagueService sportradarLeagueService;
        private const int DelayFetchStandingMinute = 0;

        public FetchLeagueStandingsTask(
            IBus messageBus,
            Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.messageBus = messageBus;
            sportradarLeagueService = leagueServiceFactory(DataProviderType.SportRadar);
        }

        public void FetchClosedMatchesStanding(IEnumerable<Match> closedMatches, Language language)
        {
            var leagues = closedMatches.Select(closedMatch => closedMatch.League).GroupBy(league => league.Id).Select(group => group.First());

            foreach (var league in leagues)
            {
                BackgroundJob.Enqueue<IFetchLeagueStandingsTask>(task => task.FetchLeagueStandings(league.Id, league.Region, language));
            }
        }

        public async Task FetchLeagueStandings(string leagueId, string region, Language language)
        {
            var leagueTables = await sportradarLeagueService.GetLeagueStandings(leagueId, language, region);
            foreach (var leagueTable in leagueTables)
            {
                await messageBus.Publish<ILeagueStandingFetchedMessage>(
                        new LeagueStandingFetchedMessage(leagueTable, language.DisplayName));
            }
        }
    }
}