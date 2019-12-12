using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
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
        Task FetchLeagueStandings(string leagueId, string region, Language language, bool getLiveDataFirst);

        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        void FetchClosedMatchesStanding(IEnumerable<Match> closedMatches, Language language);

        [Queue("medium")]
        Task FetchLeagueStandings();
    }

    public class FetchLeagueStandingsTask : IFetchLeagueStandingsTask
    {
        private readonly TimeSpan ReUpdateLeagueStandingTimeSpan = new TimeSpan(0, 10, 0);
        private readonly IBus messageBus;
        private readonly Func<DataProviderType, ILeagueService> leagueServiceFactory;
        private readonly ILeagueService sportradarLeagueService;
        private readonly IBackgroundJobClient jobClient;

        public FetchLeagueStandingsTask(
            IBus messageBus,
            Func<DataProviderType, ILeagueService> leagueServiceFactory,
            IBackgroundJobClient jobClient)
        {
            this.messageBus = messageBus;
            this.leagueServiceFactory = leagueServiceFactory;
            sportradarLeagueService = leagueServiceFactory(DataProviderType.SportRadar);

            this.jobClient = jobClient;
        }

        public void FetchClosedMatchesStanding(IEnumerable<Match> closedMatches, Language language)
        {
            if (!closedMatches.Any())
            {
                return;
            }

            var leagues = closedMatches
                .Select(closedMatch => closedMatch.League)
                .GroupBy(league => league.Id)
                .Select(group => group.First());

            foreach (var league in leagues)
            {
                BackgroundJob.Enqueue<IFetchLeagueStandingsTask>(task => task.FetchLeagueStandings(league.Id, league.Region, language, true));
                jobClient.Schedule<IFetchLeagueStandingsTask>(task => task.FetchLeagueStandings(league.Id, league.Region, language, false), ReUpdateLeagueStandingTimeSpan);
            }
        }

        public async Task FetchLeagueStandings(string leagueId, string region, Language language, bool getLiveDataFirst)
        {
            var leagueTables = await sportradarLeagueService.GetLeagueStandings(leagueId, language, region, getLiveDataFirst);

            foreach (var leagueTable in leagueTables)
            {
                await messageBus.Publish<ILeagueStandingFetchedMessage>(
                        new LeagueStandingFetchedMessage(leagueTable, language.DisplayName));
            }
        }

        public async Task FetchLeagueStandings()
        {
            var leagues = await leagueServiceFactory(DataProviderType.Internal).GetLeagues(Language.en_US);

            if (leagues != null && leagues.Any())
            {
                foreach (var league in leagues)
                {
                    await FetchLeagueStandings(league.Id, league.Region, Language.en_US, true);
                }
            }
        }
    }
}