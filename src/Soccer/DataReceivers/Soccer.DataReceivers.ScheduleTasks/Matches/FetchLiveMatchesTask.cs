namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
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
                if (match.MatchResult.EventStatus.IsClosed())
                {
                    await PublishClosedMatchMessage(match);
                }
                else
                {
                    if (match.MatchResult.EventStatus.IsLive())
                    {
                        await PublishLiveMatchMessage(match);
                    }
                }

                //TODO trigger job to fetch timeline
            }
        }

        private async Task PublishClosedMatchMessage(Match match)
            => await messageBus.Publish<ILiveMatchClosedMessage>(
                new LiveMatchClosedMessage(match.Id, match.MatchResult));

        private async Task PublishLiveMatchMessage(Match match)
            => await messageBus.Publish<ILiveMatchUpdatedMessage>(
                                new LiveMatchUpdatedMessage(match.Id, match.MatchResult));
    }
}