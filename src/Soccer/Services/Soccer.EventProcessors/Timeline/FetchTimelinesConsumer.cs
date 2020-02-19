using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using MassTransit;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timelines.QueueMessages;

namespace Soccer.EventProcessors.Timeline
{
    public class FetchTimelinesConsumer : IConsumer<IMatchTimelinesFetchedMessage>
    {
        private readonly IBus messageBus;

        public FetchTimelinesConsumer(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IMatchTimelinesFetchedMessage> context)
        {
            var match = context.Message?.Match;

            if (match == null || match.TimeLines?.Any() != true)
            {
                return;
            }

            var latestTimeline = match.TimeLines.LastOrDefault(t => !t.IsShootOutInPenalty() && !t.IsScoreChangeInPenalty());

            if (latestTimeline != null)
            {
                await PublishLatestTimeline(match.League.Id, match.Id, match.MatchResult, latestTimeline);
            }

            var timelinesSkipLastAndPenalty = match.TimeLines.SkipLast(1).Where(t => !t.IsShootOutInPenalty() && !t.IsScoreChangeInPenalty()).ToList();

            await ProcessTimelinesNotInPenalty(match, timelinesSkipLastAndPenalty);

            await ProcessPenaltyTimelines(match);

            await PublishConfirmedTimelines(match.Id, match.EventDate, match.MatchResult.EventStatus, match.TimeLines.ToList());
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

                await messageBus.Publish<IMatchEventReceivedMessage>(
                    new MatchEventReceivedMessage(new MatchEvent(match.League.Id, match.Id, match.MatchResult, timeline, false, match.EventDate)));
            }
        }

        private async Task ProcessPenaltyTimelines(Match match)
        {
            const int TwoItemsCount = 2;

            var penaltyEvents = match.TimeLines.Where(t => t.IsShootOutInPenalty()).ToList();

            if (match.MatchResult.MatchPeriods?.Any(mp => mp.PeriodType.IsPenalties()) != true)
            {
                _ = match.MatchResult.MatchPeriods.Concat(
                    new List<MatchPeriod> { new MatchPeriod { PeriodType = PeriodType.Penalties } });
            }

            await BuildShootoutPenaltyScores(match, TwoItemsCount, penaltyEvents);
        }

        private async Task BuildShootoutPenaltyScores(Match match, int TwoItemsCount, List<TimelineEvent> penaltyEvents)
        {
            var shootoutHomeScore = 0;
            var shootoutAwayScore = 0;
            var index = 0;

            while (index * TwoItemsCount < penaltyEvents.Count)
            {
                var pairOfShootoutEvents = penaltyEvents.Skip(index * TwoItemsCount).Take(TwoItemsCount).ToList();

                foreach (var shootoutEvent in pairOfShootoutEvents)
                {
                    if (shootoutEvent.IsHome)
                    {
                        shootoutHomeScore += shootoutEvent.IsHomeShootoutScored ? 1 : 0;
                    }
                    else
                    {
                        shootoutAwayScore += shootoutEvent.IsAwayShootoutScored ? 1 : 0;
                    }

                    match.MatchResult.MatchPeriods.FirstOrDefault(p => p.PeriodType.IsPenalties()).HomeScore = shootoutHomeScore;
                    match.MatchResult.MatchPeriods.FirstOrDefault(p => p.PeriodType.IsPenalties()).AwayScore = shootoutAwayScore;

                    await messageBus.Publish<IMatchEventReceivedMessage>(
                        new MatchEventReceivedMessage(new MatchEvent(match.League.Id, match.Id, match.MatchResult, shootoutEvent, false, match.EventDate)));
                }

                index++;
            }
        }

        private Task PublishLatestTimeline(string leagueId, string matchId, MatchResult matchResult, TimelineEvent latestTimeline)
        {
            var matchEvent = new MatchEvent(leagueId, matchId, matchResult, latestTimeline)
                                    .AddScoreToSpecialTimeline(matchResult);

            return messageBus.Publish<IMatchEventReceivedMessage>(new MatchEventReceivedMessage(matchEvent));
        }

        private Task PublishConfirmedTimelines(string matchId, DateTimeOffset eventDate, MatchStatus eventStatus, IList<TimelineEvent> timelines)
            => messageBus.Publish<IMatchTimelinesConfirmedMessage>(new MatchTimelinesConfirmedMessage(
                    matchId,
                    eventDate,
                    eventStatus,
                    timelines));
    }
}