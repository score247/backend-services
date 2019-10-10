﻿using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
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

            if (message == null 
                || message.Match?.TimeLines == null 
                || !message.Match.TimeLines.Any() 
                || !(await majorLeagueFilter.Filter(message.Match)))
            {
                return;
            }
         
            var latestTimeline = message.Match.TimeLines.LastOrDefault();

            var matchEvent = new MatchEvent(message.Match.League.Id, message.Match.Id, message.Match.MatchResult, latestTimeline);
            await messageBus.Publish<IMatchEventReceivedMessage>(
                new MatchEventReceivedMessage(matchEvent.AddScoreToSpecialTimeline(message.Match.MatchResult)));

            var timelinesSkipLastAndPenalty = message.Match.TimeLines.SkipLast(1).Where(t=> !t.IsShootOutInPenalty()).ToList();

            TimelineEvent latestScore = null;

            foreach (var timeline in timelinesSkipLastAndPenalty)
            {
                if (timeline.Type.IsScoreChange())
                {
                    latestScore = timeline;
                }

                //TODO update score for event penalty_missed
                if (timeline.Type.IsBreakStart() && latestScore != null)
                {
                    timeline.UpdateScore(latestScore.HomeScore, latestScore.AwayScore);
                }

                //TODO should process languages
                await messageBus.Publish<IMatchEventReceivedMessage>(
                    new MatchEventReceivedMessage(new MatchEvent(message.Match.League.Id, message.Match.Id, message.Match.MatchResult, timeline)));
            }

            //TODO need to process penalty
        }
    }
}