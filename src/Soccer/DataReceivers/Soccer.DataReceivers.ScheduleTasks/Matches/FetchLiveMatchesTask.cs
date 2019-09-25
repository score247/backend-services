namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchLiveMatchesTask
    {
        Task FetchLiveMatches();
    }

    public class FetchLiveMatchesTask : IFetchLiveMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchLiveMatchesTask(
            IBus messageBus,
            IMatchService matchService)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        /// <summary>
        ///  Only update match result so dont care about language to limit requests to sportradar
        /// </summary>
        public async Task FetchLiveMatches()
        {
            var matches = await matchService.GetLiveMatches(Language.en_US);

            foreach (var match in matches)
            {
                await PublishLiveMatchResultUpdatedMessage(match);

                StartFetchTimelinesTask(match.Id, match.Region);
            }
        }

        private async Task PublishLiveMatchResultUpdatedMessage(Match match)
            => await messageBus.Publish<ILiveMatchResultUpdatedMessage>(new LiveMatchResultUpdatedMessage(match.Id, match.MatchResult));

        private static void StartFetchTimelinesTask(string matchId, string region)
        {
            BackgroundJob.Enqueue<IFetchTimelineTask>(t => t.FetchTimelines(matchId, region));
        }
    }
}