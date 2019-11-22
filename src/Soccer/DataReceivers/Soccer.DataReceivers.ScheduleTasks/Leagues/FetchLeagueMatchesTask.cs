using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeagueMatchesTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchLeagueMatches();

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchLeagueMatches(IList<LeagueSeasonProcessedInfo> leagueSeasons);
    }
    public class FetchLeagueMatchesTask : IFetchLeagueMatchesTask
    {
        private const int NumberOfFetchInTask = 5;

        private readonly IAppSettings appSettings;
        private readonly IBus messageBus;

        private readonly ILeagueScheduleService leagueScheduleService;
        private readonly ILeagueSeasonService leagueSeasonService;
        private readonly IBackgroundJobClient jobClient;

        public FetchLeagueMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            ILeagueScheduleService leagueScheduleService,
            ILeagueSeasonService leagueSeasonService,
            IBackgroundJobClient jobClient)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.leagueScheduleService = leagueScheduleService;
            this.leagueSeasonService = leagueSeasonService;
            this.jobClient = jobClient;
        }

        public async Task FetchLeagueMatches()
        {
            var unprocessedLeagueSeason = await leagueSeasonService.GetUnprocessedLeagueSeason();

            if (unprocessedLeagueSeason?.Any() == false)
            {
                return;
            }

            for (var i = 0; i * NumberOfFetchInTask < unprocessedLeagueSeason.Count(); i++)
            {
                var batchOfLeague = unprocessedLeagueSeason.Skip(i * NumberOfFetchInTask).Take(NumberOfFetchInTask).ToList();

                jobClient.Enqueue<IFetchLeagueMatchesTask>(t => t.FetchLeagueMatches(batchOfLeague));
            }
        }

        public async Task FetchLeagueMatches(IList<LeagueSeasonProcessedInfo> leagueSeasons)
        {
            foreach (var season in leagueSeasons)
            {
                foreach (var language in Enumeration.GetAll<Language>())
                {
                    var matches = await leagueScheduleService.GetLeagueMatches(season.Region, season.LeagueId, language);
                    await PublishPreMatchesMessage(language, matches);

                    //TODO schedule to fetch timelines, lineups, team results
                }
            }
        }

        private async Task PublishPreMatchesMessage(Language language, IEnumerable<Match> matches)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;

            for (var i = 0; i * batchSize < matches.Count(); i++)
            {
                var batchOfMatches = matches.Skip(i * batchSize).Take(batchSize).ToList();

                await messageBus.Publish<IPreMatchesFetchedMessage>(
                    new PreMatchesFetchedMessage(batchOfMatches, language.DisplayName));
            }
        }
    }
}
