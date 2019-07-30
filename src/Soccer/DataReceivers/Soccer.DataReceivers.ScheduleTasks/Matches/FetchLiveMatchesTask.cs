namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchLiveMatchesTask
    {
        void FetchLiveMatches();

        Task FetchLiveMatches(Language language);
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

        public void FetchLiveMatches()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                BackgroundJob.Enqueue(() => FetchLiveMatches(language));
            }
        }

        public async Task FetchLiveMatches(Language language)
        {
            var matches = await matchService.GetLiveMatches(language);

            foreach (var match in matches)
            {
                if (match.MatchResult.EventStatus.IsClosed())
                {
                    await PublishClosedMatchMessage(match, language);
                }
                else if (match.MatchResult.EventStatus.IsLive())
                {
                    await PublishLiveMatchMessage(match, language);
                }
            }
        }

        private async Task PublishClosedMatchMessage(Match match, Language language)
            => await messageBus.Publish<ILiveMatchUpdatedToClosedEvent>(
                new LiveMatchUpdatedToClosedEvent(match.Id, language, match.MatchResult));

        private async Task PublishLiveMatchMessage(Match match, Language language)
            => await messageBus.Publish<ILiveMatchUpdatedEvent>(
                                new LiveMatchUpdatedEvent(match.Id, language));
    }
}