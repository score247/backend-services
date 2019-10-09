using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timeline.QueueMessages;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    [Trait("Soccer.EventProcessors", "FetchTimelineConsumer")]
    public class FetchTimelineConsumerTests
    {
        private readonly IBus messageBus;
        private readonly IAsyncFilter<Match, bool> majorLeagueFilter;

        private readonly ConsumeContext<IMatchTimelinesFetchedMessage> context;
        private readonly FetchTimelinesConsumer fetchTimelineConsumer;

        public FetchTimelineConsumerTests()
        {
            messageBus = Substitute.For<IBus>();
            majorLeagueFilter = Substitute.For<IAsyncFilter<Match, bool>>();

            context = Substitute.For<ConsumeContext<IMatchTimelinesFetchedMessage>>();

            fetchTimelineConsumer = new FetchTimelinesConsumer(messageBus, majorLeagueFilter);
        }

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotPublishMatchEvent()
        {
            await fetchTimelineConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_MatchNotInMajorLeagues_ShouldNotPublishMatchEvent()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match 
                {
                    League = new League { Id = "league:1" },
                    TimeLines = new List<TimelineEvent> 
                    {
                        new TimelineEvent{ Type = EventType.MatchStarted }
                    }
                }, 
                Language.en_US));

            await fetchTimelineConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_MatchInMajorLeagues_ShouldPublishMatchEventWithUpdatedScore()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 2 },
                    TimeLines = new List<TimelineEvent> 
                    { 
                        new TimelineEvent{ Type = EventType.MatchEnded }
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(
                Arg.Is<MatchEventReceivedMessage>(m => m.MatchEvent.Timeline.HomeScore == 1 && m.MatchEvent.Timeline.AwayScore == 2 ));            
        }

        [Fact]
        public async Task Consume_BreakStartEvent_ShouldPublishTimelineUpdatedMessage()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 2 },
                    TimeLines = new List<TimelineEvent>
                    {
                        new TimelineEvent{ Type = EventType.BreakStart },
                        new TimelineEvent{ Type = EventType.MatchEnded }
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<ITimelineUpdatedMessage>(Arg.Any<TimelineUpdatedMessage>());
        }

        [Fact]
        public async Task Consume_BreakStartEventAndScoreChanged_ShouldPublishTimelineUpdatedMessage()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        new TimelineEvent{ Type = EventType.MatchStarted },
                        new TimelineEvent{ Type = EventType.ScoreChange, HomeScore = 1, AwayScore = 0, Time = DateTime.Now.AddMinutes(-1) },
                        new TimelineEvent{ Type = EventType.BreakStart, Time = DateTime.Now },
                        new TimelineEvent{ Type = EventType.ScoreChange, HomeScore = 1, AwayScore = 1, Time = DateTime.Now.AddMinutes(1) },
                        new TimelineEvent{ Type = EventType.MatchEnded }
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<ITimelineUpdatedMessage>(
                Arg.Is<TimelineUpdatedMessage>(m => m.Timeline.Type.IsBreakStart() && m.Timeline.HomeScore == 1 && m.Timeline.AwayScore == 0));
        }

        [Fact]
        public async Task Consume_TimelineEvents_ShouldPublishTimelineUpdatedMessageSkipLast()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        new TimelineEvent{ Type = EventType.ScoreChange, HomeScore = 1, AwayScore = 0 },
                        new TimelineEvent{ Type = EventType.YellowCard },
                        new TimelineEvent{ Type = EventType.RedCard },
                        new TimelineEvent{ Type = EventType.Substitution },
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(3).Publish<ITimelineUpdatedMessage>(Arg.Any<TimelineUpdatedMessage>());
        }
    }
}
