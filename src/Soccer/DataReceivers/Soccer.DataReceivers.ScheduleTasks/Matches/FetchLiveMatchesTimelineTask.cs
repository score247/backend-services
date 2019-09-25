namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using Hangfire;
    using MassTransit;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using System.Threading.Tasks;

    public interface IFetchLiveMatchesTimelineTask
    {
        Task FetchLiveMatches();
    }

    public class FetchLiveMatchesTimelineTask : IFetchLiveMatchesTimelineTask
    {
        private readonly IMatchService matchService;        

        public FetchLiveMatchesTimelineTask(
            IBus messageBus,
            IMatchService matchService)
        {            
            this.matchService = matchService;
        }

        public async Task FetchLiveMatches()
        {
            var matches = await matchService.GetLiveMatches(Language.en_US);

            foreach (var match in matches)
            {
                BackgroundJob.Enqueue<IFetchTimelineTask>(t => t.FetchTimelines(match.Id, match.Region));
            }
        }
    }
}