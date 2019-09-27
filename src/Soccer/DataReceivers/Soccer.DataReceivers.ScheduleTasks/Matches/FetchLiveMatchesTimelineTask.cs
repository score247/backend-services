namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchLiveMatchesTimelineTask
    {
        Task FetchLiveMatches();
    }

    public class FetchLiveMatchesTimelineTask : IFetchLiveMatchesTimelineTask
    {
        private readonly IMatchService matchService;

        public FetchLiveMatchesTimelineTask(
            IMatchService matchService)
        {
            this.matchService = matchService;
        }

        public async Task FetchLiveMatches()
        {
            //TODO should support multiple languages
            var matches = await matchService.GetLiveMatches(Language.en_US);

            foreach (var match in matches)
            {
                BackgroundJob.Enqueue<IFetchTimelineTask>(t => t.FetchTimelines(match.Id, match.Region));
            }
        }
    }
}