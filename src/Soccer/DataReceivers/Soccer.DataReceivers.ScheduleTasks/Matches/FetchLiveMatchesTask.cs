namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.Matches.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IFetchLiveMatchesTask
    {
        Task FetchPostMatches(int dateSpan);
    }

    public class FetchLiveMatchesTask : IFetchLiveMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchLiveMatchesTask(IBus messageBus, IMatchService matchService)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public async Task FetchPostMatches(int dateSpan)
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                var matches = await matchService.GetLiveMatches(language);

                var liveMatches = matches.Where(x => x.MatchResult.EventStatus == MatchStatus.Live);
                var closedMatches = matches.Where(x => x.MatchResult.EventStatus == MatchStatus.Closed);

                if (closedMatches.Any())
                {
                    await PublishClosedMatchEvents(closedMatches, language);
                }

                if (liveMatches.Any())
                {                    
                    await messageBus.Publish<ILiveMatchUpdatedEvent>(new { Matches = matches, Language = language.DisplayName });
                }
            }
        }

        private async Task PublishClosedMatchEvents(IEnumerable<Match> matches, Language language)
        {
            foreach (var match in matches)
            {
                await messageBus.Publish<ILiveMatchUpdatedToClosedEvent>(new
                {
                    MatchId = match.Id,
                    match.MatchResult.MatchStatus,
                    match.MatchResult.EventStatus,
                    Language = language.DisplayName
                });
            }
        }
    }
}
