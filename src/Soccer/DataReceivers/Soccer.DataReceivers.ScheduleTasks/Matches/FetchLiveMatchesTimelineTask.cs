using System;
using System.Linq;
using System.Threading.Tasks;
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
        [Queue("mediumlive")]
        Task FetchLiveMatchesTimeline();
    }

    public class FetchLiveMatchesTimelineTask : IFetchLiveMatchesTimelineTask
    {
        private readonly IMatchService matchService;
        private readonly IFetchMatchLineupsTask fetchMatchLineupsTask;
        private readonly IFetchTimelineTask fetchTimelineTask;
        private readonly ILeagueService internalLeagueService;

        public FetchLiveMatchesTimelineTask(
            IMatchService matchService,
            IFetchMatchLineupsTask fetchMatchLineupsTask,
            IFetchTimelineTask fetchTimelineTask,
            Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.matchService = matchService;
            this.fetchMatchLineupsTask = fetchMatchLineupsTask;
            this.fetchTimelineTask = fetchTimelineTask;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
        }

        public async Task FetchLiveMatchesTimeline()
        {
            var majorLeagues = await internalLeagueService.GetLeagues(Language.en_US);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                var matches = (await matchService.GetLiveMatches(Language.en_US))
                    .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true);

                foreach (var match in matches)
                {
                    await Task.WhenAll(
                        fetchTimelineTask.FetchTimelines(match.Id, match.Region, language),
                        fetchMatchLineupsTask.FetchMatchLineups(match.Id, match.Region, language));
                }
            }
        }
    }
}