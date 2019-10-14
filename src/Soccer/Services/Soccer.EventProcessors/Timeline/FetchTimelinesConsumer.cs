using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Timeline
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
            var match = context.Message?.Match;

            if (match == null
                || match.TimeLines == null
                || !match.TimeLines.Any()
                || !(await majorLeagueFilter.Filter(match)))
            {
                return;
            }

            var latestTimeline = match.TimeLines.LastOrDefault();
            await PublishLatestTimeline(match.League.Id, match.Id, match.MatchResult, latestTimeline);

            var timelinesSkipLastAndPenalty = match.TimeLines.SkipLast(1).Where(t => !t.IsShootOutInPenalty()).ToList();

            TimelineEvent latestScore = null;

            foreach (var timeline in timelinesSkipLastAndPenalty)
            {
                if (timeline.Type.IsScoreChange())
                {
                    latestScore = timeline;
                }

                if (timeline.ShouldReprocessScore() && latestScore != null)
                {
                    timeline.UpdateScore(latestScore.HomeScore, latestScore.AwayScore);
                }

                //TODO should process languages
                await messageBus.Publish<IMatchEventReceivedMessage>(
                    new MatchEventReceivedMessage(new MatchEvent(match.League.Id, match.Id, match.MatchResult, timeline, false)));
            }

            //TODO need to process penalty
        }

        private Task PublishLatestTimeline(string leagueId, string matchId, MatchResult matchResult, TimelineEvent latestTimeline)
        {
            var matchEvent = new MatchEvent(leagueId, matchId, matchResult, latestTimeline)
                                    .AddScoreToSpecialTimeline(matchResult);

            return messageBus.Publish<IMatchEventReceivedMessage>(new MatchEventReceivedMessage(matchEvent));
        }
    }
}
