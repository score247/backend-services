using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Timeline;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timeline
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
        public async Task Consume_MatchInMajorLeagues_ShouldPublishLastEventWithUpdatedScore()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 2 },
                    TimeLines = new List<TimelineEvent>
                    {
                        new TimelineEvent{ Type = EventType.MatchStarted },
                        new TimelineEvent{ Type = EventType.MatchEnded }
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(
                Arg.Is<MatchEventReceivedMessage>(m =>
                    m.MatchEvent.Timeline.Type.IsMatchEnd()
                    && m.MatchEvent.Timeline.HomeScore == 1
                    && m.MatchEvent.Timeline.AwayScore == 2));
        }

        [Fact]
        public async Task Consume_BreakStartEvent_ShouldPublishMatchEvent()
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

            await messageBus.Received(2).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_BreakStartEventAndScoreChanged_ShouldUpdatedScore()
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

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(
                Arg.Is<MatchEventReceivedMessage>(m => m.MatchEvent.Timeline.Type.IsBreakStart()
                    && m.MatchEvent.Timeline.HomeScore == 1
                    && m.MatchEvent.Timeline.AwayScore == 0));
        }

        [Fact]
        public async Task Consume_TimelineEvents_ShouldPublishMatchEvent()
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

            await messageBus.Received(4).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_FirstHomePenaltyShootoutEvents_ShouldPublishMatchEvent()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        StubHomePenaltyShootout("1", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0)
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventReceivedMessage>(
                m => m.MatchEvent.Timeline.Type.IsPenaltyShootout() &&
                    m.MatchEvent.Timeline.IsHomeShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutHomeScore == 1 &&
                    !m.MatchEvent.Timeline.IsAwayShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutAwayScore == 0 &&
                    m.MatchEvent.Timeline.IsFirstShoot));
        }

        [Fact]
        public async Task Consume_FirstAwayPenaltyShootoutEvents_ShouldPublishMatchEvent()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        StubAwayPenaltyShootout("1", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0)
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventReceivedMessage>(
                m => m.MatchEvent.Timeline.Type.IsPenaltyShootout() &&
                    m.MatchEvent.Timeline.IsAwayShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutAwayScore == 1 &&
                    !m.MatchEvent.Timeline.IsHomeShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutHomeScore == 0 &&
                    m.MatchEvent.Timeline.IsFirstShoot));
        }

        [Fact]
        public async Task Consume_PairOfPenaltyShootoutEventsAndHomeFirst_ShouldPublishMatchEvent()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        StubHomePenaltyShootout("1", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),
                        StubAwayPenaltyShootout("2", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 1),
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventReceivedMessage>(
                m => m.MatchEvent.Timeline.Type.IsPenaltyShootout()
                    && m.MatchEvent.Timeline.IsHomeShootoutScored 
                    && m.MatchEvent.Timeline.ShootoutHomeScore == 1 
                    && m.MatchEvent.Timeline.IsAwayShootoutScored 
                    && m.MatchEvent.Timeline.ShootoutAwayScore == 1 
                    && !m.MatchEvent.Timeline.IsFirstShoot));
        }

        [Fact]
        public async Task Consume_PenaltyShootoutEventsAndAwayFirst_ShouldPublishMatchEvent()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        StubAwayPenaltyShootout("1", scored: false),
                        StubHomePenaltyShootout("2", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),                        
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventReceivedMessage>(
                m => m.MatchEvent.Timeline.Type.IsPenaltyShootout() &&
                    m.MatchEvent.Timeline.IsHomeShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutHomeScore == 1 &&
                    !m.MatchEvent.Timeline.IsAwayShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutAwayScore == 0 &&
                    !m.MatchEvent.Timeline.IsFirstShoot));
        }

        [Fact]
        public async Task Consume_PenaltyShootoutEvents_ShouldPublishMatchEvent()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        StubAwayPenaltyShootout("1", scored: false),
                        StubHomePenaltyShootout("2", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),

                        StubAwayPenaltyShootout("3", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 1),
                        StubHomePenaltyShootout("4", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 2, shootoutAwayScore: 1),

                        StubAwayPenaltyShootout("5", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 2, shootoutAwayScore: 2),
                        StubHomePenaltyShootout("6", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 3, shootoutAwayScore: 2),

                        StubAwayPenaltyShootout("7", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 3, shootoutAwayScore: 3),
                        StubHomePenaltyShootout("8", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 4, shootoutAwayScore: 3),

                        StubAwayPenaltyShootout("9", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 4, shootoutAwayScore: 4),
                        StubHomePenaltyShootout("10", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 5, shootoutAwayScore: 4),
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(10).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventReceivedMessage>(
                m => m.MatchEvent.Timeline.Type.IsPenaltyShootout() &&
                    m.MatchEvent.Timeline.IsHomeShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutHomeScore == 5 &&
                    m.MatchEvent.Timeline.IsAwayShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutAwayScore == 4 &&
                    !m.MatchEvent.Timeline.IsFirstShoot));
        }

        [Fact]
        public async Task Consume_PenaltyHomeScored4AndAwayScored3_ShouldPublishLatestMatchEventWithFinalScore()
        {
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                new Match
                {
                    League = new League { Id = "league:1" },
                    MatchResult = new MatchResult { HomeScore = 1, AwayScore = 1 },
                    TimeLines = new List<TimelineEvent>
                    {
                        StubHomePenaltyShootout("1", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),
                        StubAwayPenaltyShootout("2", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 1),

                        StubHomePenaltyShootout("3", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 2, shootoutAwayScore: 1),
                        StubAwayPenaltyShootout("4", scored: false),

                        StubHomePenaltyShootout("5", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 3, shootoutAwayScore: 1),
                        StubAwayPenaltyShootout("6", scored: false),

                        StubHomePenaltyShootout("7", scored: true),
                        StubScoreChangeInPenalty(shootoutHomeScore: 4, shootoutAwayScore: 1),
                    }
                },
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(7).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventReceivedMessage>(
                m => m.MatchEvent.Timeline.Type.IsPenaltyShootout() &&
                    m.MatchEvent.Timeline.IsHomeShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutHomeScore == 4 &&
                    !m.MatchEvent.Timeline.IsAwayShootoutScored &&
                    m.MatchEvent.Timeline.ShootoutAwayScore == 1 &&
                    m.MatchEvent.Timeline.IsFirstShoot));
        }

        private static TimelineEvent StubHomePenaltyShootout(string id, bool scored)
            => StubTimeline(id, true, scored);

        private static TimelineEvent StubAwayPenaltyShootout(string id, bool scored)
            => StubTimeline(id, false, scored);

        private static TimelineEvent StubScoreChangeInPenalty(int shootoutHomeScore, int shootoutAwayScore)
            => new  TimelineEvent 
                { 
                    Type = EventType.ScoreChange, 
                    PeriodType = PeriodType.Penalties, 
                    ShootoutHomeScore = shootoutHomeScore, 
                    ShootoutAwayScore = shootoutAwayScore
                };
        

        private static TimelineEvent StubTimeline(string id, bool isHome, bool isScored, string playerName = "playerName")
            => new TimelineEvent
            {
                Id = id,
                Type = EventType.PenaltyShootout,
                Team = isHome ? "home" : "away",
                IsHomeShootoutScored = isHome && isScored,
                IsAwayShootoutScored = !isHome && isScored,
                PeriodType = PeriodType.Penalties,          
                Player = new Player
                {
                    Id = "player_id",
                    Name = playerName
                }
            };
    }
}
