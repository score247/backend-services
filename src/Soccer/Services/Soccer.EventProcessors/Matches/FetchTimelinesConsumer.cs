using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
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

            var timelinesSkipLast = message.Match.TimeLines.SkipLast(1).ToList();

            TimelineEvent latestScore = null;

            foreach (var timeline in timelinesSkipLast)
            {
                if (timeline.Type.IsScoreChange())
                {
                    latestScore = timeline;
                }

                if (timeline.Type.IsBreakStart() && latestScore != null)
                {
                    timeline.UpdateScore(latestScore.HomeScore, latestScore.AwayScore);
                }

                await messageBus.Publish<ITimelineUpdatedMessage>(
                    new TimelineUpdatedMessage(message.Match.Id, message.Match.League.Id, message.Language, timeline));
            }
        }
    }
}
