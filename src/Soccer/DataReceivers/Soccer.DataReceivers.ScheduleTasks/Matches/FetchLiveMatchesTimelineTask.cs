namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchLiveMatchesTimelineTask
    {
        [Queue("medium")]
        Task FetchLiveMatchesTimeline();
    }

    public class FetchLiveMatchesTimelineTask : IFetchLiveMatchesTimelineTask
    {
        private readonly IMatchService matchService;

        public FetchLiveMatchesTimelineTask(
            IMatchService matchService)
        {
            this.matchService = matchService;
        }

        public async Task FetchLiveMatchesTimeline()
        {
            var matches = await matchService.GetLiveMatches(Language.en_US);

            foreach (var match in matches)
            {
                BackgroundJob.Enqueue<IFetchTimelineTask>(t => t.FetchTimelines(match.Id, match.Region));
            }
        }
    }
}