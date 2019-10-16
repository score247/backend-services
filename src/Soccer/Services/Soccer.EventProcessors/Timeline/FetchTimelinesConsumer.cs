using System.Collections.Generic;
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

            //TODO process latest in case of penalty shootout
            var latestTimeline = match.TimeLines.LastOrDefault(t => !t.IsShootOutInPenalty() && !t.IsScoreChangeInPenalty());

            if (latestTimeline != null)
            {
                await PublishLatestTimeline(match.League.Id, match.Id, match.MatchResult, latestTimeline);
            }

            var timelinesSkipLastAndPenalty = match.TimeLines.SkipLast(1).Where(t => !t.IsShootOutInPenalty() && !t.IsScoreChangeInPenalty()).ToList();

            await ProcessTimelinesNotInPenalty(match, timelinesSkipLastAndPenalty);

            await ProcessPenaltyTimelines(match);
        }

        private async Task ProcessTimelinesNotInPenalty(Match match, List<TimelineEvent> timelinesSkipLastAndPenalty)
        {
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
        }

        private async Task ProcessPenaltyTimelines(Match match)
        {
            const int TwoItemsCount = 2;
            var index = 0;
            var shootoutHomeScore = 0;
            var shootoutAwayScore = 0;

            var penaltyEvents = match.TimeLines.Where(t => t.IsShootOutInPenalty()).ToList();

            while (index * TwoItemsCount < penaltyEvents.Count)
            {
                var pair = penaltyEvents.Skip(index * TwoItemsCount).Take(TwoItemsCount).ToList();

                var firstShootoutEvent = await ProcessAndPublishFirstShootout(shootoutHomeScore, shootoutAwayScore, match, pair[0]);

                shootoutHomeScore = firstShootoutEvent.ShootoutHomeScore;
                shootoutAwayScore = firstShootoutEvent.ShootoutAwayScore;

                if (pair.Count > 1)
                {
                    var lastShootoutEvent = await ProcessAndPublishLastShootout(shootoutHomeScore, shootoutAwayScore, match, pair[1], firstShootoutEvent);

                    shootoutHomeScore = lastShootoutEvent.ShootoutHomeScore;
                    shootoutAwayScore = lastShootoutEvent.ShootoutAwayScore;
                }

                index++;
            }
        }

        private async Task<TimelineEvent> ProcessAndPublishLastShootout(
            int shootoutHomeScore,
            int shootoutAwayScore,
            Match match,
            TimelineEvent lastShootout,
            TimelineEvent firstShootoutEvent)
        {
            var lastShootoutEvent = HandlePairOfPenaltyEvents(lastShootout, firstShootoutEvent, shootoutHomeScore, shootoutAwayScore);

            await messageBus.Publish<IMatchEventReceivedMessage>(
                new MatchEventReceivedMessage(new MatchEvent(match.League.Id, match.Id, match.MatchResult, lastShootoutEvent, false)));

            return lastShootoutEvent;
        }

        private async Task<TimelineEvent> ProcessAndPublishFirstShootout(
            int shootoutHomeScore,
            int shootoutAwayScore,
            Match match,
            TimelineEvent firstShootout)
        {
            var firstShootoutEvent = GetShootoutEvent(firstShootout, true, shootoutHomeScore, shootoutAwayScore);

            await messageBus.Publish<IMatchEventReceivedMessage>(
                    new MatchEventReceivedMessage(new MatchEvent(match.League.Id, match.Id, match.MatchResult, firstShootoutEvent, false)));

            return firstShootoutEvent;
        }

        private static TimelineEvent HandlePairOfPenaltyEvents(
            TimelineEvent secondShootout,
            TimelineEvent firstShootout,
            int shootoutHomeScore = 0,
            int shootoutAwayScore = 0)
        {
            var shootoutEvent = GetShootoutEvent(secondShootout, false, shootoutHomeScore, shootoutAwayScore);

            if (shootoutEvent.IsHome && !firstShootout.IsHome)
            {
                shootoutEvent.AwayShootoutPlayer = firstShootout.AwayShootoutPlayer;
                shootoutEvent.IsAwayShootoutScored = firstShootout.IsAwayShootoutScored;

                shootoutEvent.HomeShootoutPlayer = shootoutEvent.Player;
            }
            else
            {
                shootoutEvent.HomeShootoutPlayer = firstShootout.HomeShootoutPlayer;
                shootoutEvent.IsHomeShootoutScored = firstShootout.IsHomeShootoutScored;

                shootoutEvent.AwayShootoutPlayer = shootoutEvent.Player;
            }

            return shootoutEvent;
        }

        private static TimelineEvent GetShootoutEvent(TimelineEvent shootoutEvent, bool isFirstShoot, int shootoutHomeScore = 0, int shootoutAwayScore = 0)
        {
            shootoutEvent.IsFirstShoot = isFirstShoot;

            if (shootoutEvent.IsHome)
            {
                shootoutEvent.HomeShootoutPlayer = shootoutEvent.Player;
                shootoutEvent.ShootoutHomeScore = shootoutHomeScore + (shootoutEvent.IsHomeShootoutScored ? 1 : 0);
                shootoutEvent.ShootoutAwayScore = shootoutAwayScore;
            }
            else
            {
                shootoutEvent.AwayShootoutPlayer = shootoutEvent.Player;
                shootoutEvent.ShootoutAwayScore = shootoutAwayScore + (shootoutEvent.IsAwayShootoutScored ? 1 : 0);
                shootoutEvent.ShootoutHomeScore = shootoutHomeScore;
            }

            return shootoutEvent;
        }

        private Task PublishLatestTimeline(string leagueId, string matchId, MatchResult matchResult, TimelineEvent latestTimeline)
        {
            var matchEvent = new MatchEvent(leagueId, matchId, matchResult, latestTimeline)
                                    .AddScoreToSpecialTimeline(matchResult);

            return messageBus.Publish<IMatchEventReceivedMessage>(new MatchEventReceivedMessage(matchEvent));
        }
    }
}
