using System;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Hangfire;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchLiveMatchesTimelineTask
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
#pragma warning disable S109 // Magic numbers should not be used
        [DisableConcurrentExecution(timeoutInSeconds: 240)]
#pragma warning restore S109 // Magic numbers should not be used
        [Queue("mediumlive")]
        Task FetchLiveMatchesTimeline();
    }

    public class FetchLiveMatchesTimelineTask : IFetchLiveMatchesTimelineTask
    {
        private readonly IMatchService matchService;
        private readonly IFetchMatchLineupsTask fetchMatchLineupsTask;
        private readonly IFetchTimelineTask fetchTimelineTask;
        private readonly ILeagueService internalLeagueService;
        private readonly ILogger logger;

        public FetchLiveMatchesTimelineTask(
            IMatchService matchService,
            IFetchMatchLineupsTask fetchMatchLineupsTask,
            IFetchTimelineTask fetchTimelineTask,
            Func<DataProviderType, ILeagueService> leagueServiceFactory,
            ILogger logger)
        {
            this.matchService = matchService;
            this.fetchMatchLineupsTask = fetchMatchLineupsTask;
            this.fetchTimelineTask = fetchTimelineTask;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
            this.logger = logger;
        }

        public async Task FetchLiveMatchesTimeline()
        {
            var majorLeagues = (await internalLeagueService.GetLeagues(Language.en_US))?.ToList();

            if (majorLeagues?.Any() != true)
            {
                await logger.ErrorAsync("FetchLiveMatchesTimeline - Major leagues not found");
                return;
            }

            foreach (var language in Enumeration.GetAll<Language>())
            {
                var matches = (await matchService.GetLiveMatches(Language.en_US))
                    .Where(match => majorLeagues.Any(league => league.Id == match.League.Id));

                foreach (var match in matches)
                {
                    await fetchTimelineTask.FetchTimelineEvents(match.Id, match.Region, language);

                    if (match.MatchResult?.EventStatus?.IsClosed() == false)
                    {
                        await fetchMatchLineupsTask.FetchMatchLineups(match.Id, match.Region, language);
                    }
                }
            }
        }
    }
}