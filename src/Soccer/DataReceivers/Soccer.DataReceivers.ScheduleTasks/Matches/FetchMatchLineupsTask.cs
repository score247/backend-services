using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchMatchLineupsTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task FetchMatchLineups(string matchId, string region, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task FetchMatchLineups();
    }

    public class FetchMatchLineupsTask : IFetchMatchLineupsTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly ILeagueService internalLeagueService;

        public FetchMatchLineupsTask(
            IMatchService matchService,
            IBus messageBus,
            Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.matchService = matchService;
            this.messageBus = messageBus;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
        }

        public async Task FetchMatchLineups()
        {
            var majorLeagues = await internalLeagueService.GetLeagues(Language.en_US);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                var todayMatches = (await matchService.GetPreMatches(DateTime.Now.Date, language))
                    .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true);

                foreach (var match in todayMatches)
                {
                    await FetchMatchLineups(match.Id, match.Region, language);
                }
            }
        }

        public async Task FetchMatchLineups(string matchId, string region, Language language)
        {
            if (!string.IsNullOrWhiteSpace(matchId))
            {
                var matchLineups = await matchService.GetLineups(matchId, region, language);

                if (matchLineups != null
                    && !string.IsNullOrWhiteSpace(matchLineups.Id))
                {
                    await messageBus.Publish<IMatchLineupsMessage>(new MatchLineupsMessage(matchLineups, language));
                }
            }
        }
    }
}