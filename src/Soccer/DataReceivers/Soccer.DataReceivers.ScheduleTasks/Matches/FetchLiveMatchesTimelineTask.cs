namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchLiveMatchesTimelineTask
    {
        [Queue("medium-live")]
        Task FetchLiveMatchesTimeline();
    }

    public class FetchLiveMatchesTimelineTask : IFetchLiveMatchesTimelineTask
    {
        private readonly IMatchService matchService;
        private readonly IFetchMatchLineupsTask fetchMatchLineupsTask;
        private readonly IFetchTimelineTask fetchTimelineTask;

        public FetchLiveMatchesTimelineTask(
            IMatchService matchService,
            IFetchMatchLineupsTask fetchMatchLineupsTask,
            IFetchTimelineTask fetchTimelineTask)
        {
            this.matchService = matchService;
            this.fetchMatchLineupsTask = fetchMatchLineupsTask;
            this.fetchTimelineTask = fetchTimelineTask;
        }

        public async Task FetchLiveMatchesTimeline()
        {
            var matches = await matchService.GetLiveMatches(Language.en_US);

            foreach (var match in matches)
            {
                await Task.WhenAll(
                    fetchTimelineTask.FetchTimelines(match.Id, match.Region),
                    fetchMatchLineupsTask.FetchMatchLineups(match.Id, match.Region));
            }
        }
    }
}