using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timeline.QueueMessages;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Matches
{
    public class FetchTimelinesConsumer : IConsumer<IMatchTimelinesFetchedMessage>
    {
        private readonly IBus messageBus;
        private readonly IAsyncFilter<Match, bool> majorLeagueFilter;

        public FetchTimelinesConsumer(
            IBus messageBus,
            IAsyncFilter<Match, bool> majorLeagueFilter)
        {
            this.messageBus = messageBus;
            this.majorLeagueFilter = majorLeagueFilter;
        }
        
        public async Task Consume(ConsumeContext<IMatchTimelinesFetchedMessage> context)
        {
            var message = context.Message;

            if (message == null || message.Match == null || message.Match.TimeLines == null || !message.Match.TimeLines.Any())
            {
                return;
            }

            if (!(await majorLeagueFilter.Filter(message.Match)))
            {
                return;
            }

            var latestTimeline = message.Match.TimeLines.LastOrDefault();

            var matchEvent = new MatchEvent(message.Match.League.Id, message.Match.Id, message.Match.MatchResult, latestTimeline);
            await messageBus.Publish<IMatchEventReceivedMessage>(
                new MatchEventReceivedMessage(matchEvent.AddScoreToSpecialTimeline(message.Match.MatchResult)));
            
            if (!message.Match.TimeLines.SkipLast(1).Any()) 
            {
                return;
            }

            await ProcessBreakStartEvent(message.Match.League.Id, message.Match.Id, message.Language, message.Match.TimeLines.ToList());

            var filteredTimelinesButLast = message.Match.TimeLines
                .SkipLast(1)
                .Where(t => !t.Type.IsBreakStart())
                .ToList();

            foreach (var timeline in filteredTimelinesButLast)
            {
                await messageBus.Publish<ITimelineUpdatedMessage>(
                    new TimelineUpdatedMessage(message.Match.Id, message.Match.League.Id, message.Language, timeline));
            }
        }

        //TODO should separate as a PreProcsessor
        private async Task ProcessBreakStartEvent(string leagueId, string matchId, Language language, IReadOnlyList<TimelineEvent> timelines)
        {
            var breakStarts = timelines.Where(t => t.Type.IsBreakStart()).ToList();

            foreach (var breakStart in breakStarts)
            {
                var latestScore = timelines
                    .LastOrDefault(t => t.Type.IsScoreChange() && t.Time < breakStart.Time);

                breakStart.HomeScore = latestScore == null ? 0 : latestScore.HomeScore;
                breakStart.AwayScore = latestScore == null ? 0 : latestScore.AwayScore;
                
                await messageBus.Publish<ITimelineUpdatedMessage>(
                        new TimelineUpdatedMessage(matchId, leagueId, language, breakStart));
            }
        }
    }
}
