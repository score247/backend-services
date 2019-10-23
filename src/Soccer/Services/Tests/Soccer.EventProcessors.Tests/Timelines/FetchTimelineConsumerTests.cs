using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
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
        private static readonly Fixture fixture = new Fixture();
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchStarted).Create()
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            await fetchTimelineConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_MatchInMajorLeagues_ShouldPublishLastEventWithUpdatedScore()
        {
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 2)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchStarted).Create(),
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchEnded).Create()
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 2)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.BreakStart).Create(),
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchEnded).Create()
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(2).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_BreakStartEventAndScoreChanged_ShouldUpdatedScore()
        {
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchStarted).Create(),
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 0)
                        .With(t => t.Time, DateTime.Now.AddMinutes(-1))
                        .Create(),
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.BreakStart)
                        .With(t => t.Time, DateTime.Now)
                        .Create(),
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 1)
                        .With(t => t.Time, DateTime.Now.AddMinutes(1))
                        .Create(),
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchEnded).Create()
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 0)
                        .Create(),
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.YellowCard)
                        .Create(),
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.RedCard)
                        .Create(),
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.Substitution).Create()
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(4).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_FirstHomePenaltyShootoutEvents_ShouldPublishMatchEvent()
        {
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubHomePenaltyShootout("1", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0)
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventProcessedMessage>(
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubAwayPenaltyShootout("1", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0)
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventProcessedMessage>(
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubHomePenaltyShootout("1", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),
                    StubAwayPenaltyShootout("2", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 1),
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventProcessedMessage>(
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubAwayPenaltyShootout("1", scored: false),
                    StubHomePenaltyShootout("2", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventProcessedMessage>(
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
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
                })
                .Create();

            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(10).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventProcessedMessage>());

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventProcessedMessage>(
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
            var match = fixture.For<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult, fixture
                    .For<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1)
                    .Create())
                .With(m => m.TimeLines, new List<TimelineEvent>
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
                })
                .Create();
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            majorLeagueFilter.Filter(Arg.Any<Match>()).Returns(true);

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(7).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventProcessedMessage>());

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(Arg.Is<MatchEventProcessedMessage>(
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
            => fixture.For<TimelineEvent>()
                .With(t => t.Type, EventType.ScoreChange)
                .With(t => t.PeriodType, PeriodType.Penalties)
                .With(t => t.ShootoutHomeScore, shootoutHomeScore)
                .With(t => t.ShootoutAwayScore, shootoutAwayScore)
                .Create();

        private static TimelineEvent StubTimeline(string id, bool isHome, bool isScored, string playerName = "playerName")
            => fixture.For<TimelineEvent>()
                .With(t => t.Id, id)
                .With(t => t.Type, EventType.PenaltyShootout)
                .With(t => t.PeriodType, PeriodType.Penalties)
                .With(t => t.Team, isHome ? "home" : "away")
                .With(t => t.Player, new Player("player_id", playerName))
                .Create();
    }
}