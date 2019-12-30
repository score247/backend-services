using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.EventProcessors.Shared.Configurations;
using Soccer.EventProcessors.Timeline;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timeline
{
    [Trait("Soccer.EventProcessors", "FetchTimelineConsumer")]
    public class FetchTimelineConsumerTests
    {
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;
        private readonly ConsumeContext<IMatchTimelinesFetchedMessage> context;
        private readonly FetchTimelinesConsumer fetchTimelineConsumer;

        public FetchTimelineConsumerTests()
        {
            messageBus = Substitute.For<IBus>();

            context = Substitute.For<ConsumeContext<IMatchTimelinesFetchedMessage>>();

            fetchTimelineConsumer = new FetchTimelinesConsumer(messageBus);
        }

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotPublishMatchEvent()
        {
            await fetchTimelineConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_MatchInMajorLeagues_ShouldPublishLastEventWithUpdatedScore()
        {
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 2))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.MatchStarted),
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.MatchEnded)
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 2))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.BreakStart),
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.MatchEnded)
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(2).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_BreakStartEventAndScoreChanged_ShouldUpdatedScore()
        {
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 1))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.MatchStarted),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 0)
                        .With(t => t.Time, DateTime.Now.AddMinutes(-1)),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.BreakStart)
                        .With(t => t.Time, DateTime.Now),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 1)
                        .With(t => t.Time, DateTime.Now.AddMinutes(1)),
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.MatchEnded)
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventReceivedMessage>(
                Arg.Is<MatchEventReceivedMessage>(m => m.MatchEvent.Timeline.Type.IsBreakStart()
                    && m.MatchEvent.Timeline.HomeScore == 1
                    && m.MatchEvent.Timeline.AwayScore == 0));
        }

        [Fact]
        public async Task Consume_TimelineEvents_ShouldPublishMatchEvent()
        {
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 1))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 0),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.YellowCard),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.RedCard),
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.Substitution)
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(4).Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>());
        }

        [Fact]
        public async Task Consume_FirstHomePenaltyShootoutEvents_ShouldPublishMatchEvent()
        {
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubHomePenaltyShootout("1", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0)
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 1))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubAwayPenaltyShootout("1", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0)
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 1))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubHomePenaltyShootout("1", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),
                    StubAwayPenaltyShootout("2", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 1),
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                    .With(r => r.HomeScore, 1)
                    .With(r => r.AwayScore, 1))
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    StubAwayPenaltyShootout("1", scored: false),
                    StubHomePenaltyShootout("2", scored: true),
                    StubScoreChangeInPenalty(shootoutHomeScore: 1, shootoutAwayScore: 0),
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 1))
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
                });

            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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
            var match = A.Dummy<Match>()
                .With(m => m.League, new League("league:1", ""))
                .With(m => m.MatchResult,
                    A.Dummy<MatchResult>()
                        .With(r => r.HomeScore, 1)
                        .With(r => r.AwayScore, 1))
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
                });
            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

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

        [Fact]
        public async Task Consume_Timelines_PublishAllTimelinesAsConfirmed()
        {
            var match = A.Dummy<Match>()
               .With(m => m.League, new League("league:1", ""))
               .With(m => m.MatchResult,
                   A.Dummy<MatchResult>()
                       .With(r => r.HomeScore, 1)
                       .With(r => r.AwayScore, 1))
               .With(m => m.TimeLines, new List<TimelineEvent>
               {
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.HomeScore, 1)
                        .With(t => t.AwayScore, 0),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.YellowCard),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.RedCard),
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.Substitution)
               });

            context.Message.Returns(new MatchTimelinesFetchedMessage(
                match,
                Language.en_US));

            await fetchTimelineConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchTimelinesConfirmedMessage>(Arg.Is<MatchTimelinesConfirmedMessage>(
                    msg => msg.Timelines.Count == match.TimeLines.Count()
                ));
        }

        private static TimelineEvent StubHomePenaltyShootout(string id, bool scored)
            => StubTimeline(id, true, scored);

        private static TimelineEvent StubAwayPenaltyShootout(string id, bool scored)
            => StubTimeline(id, false, scored);

        private static TimelineEvent StubScoreChangeInPenalty(int shootoutHomeScore, int shootoutAwayScore)
            => A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.ScoreChange)
                .With(t => t.PeriodType, PeriodType.Penalties)
                .With(t => t.ShootoutHomeScore, shootoutHomeScore)
                .With(t => t.ShootoutAwayScore, shootoutAwayScore);

        private static TimelineEvent StubTimeline(string id, bool isHome, bool isScored,
            string playerName = "playerName")
            => A.Dummy<TimelineEvent>()
                .With(t => t.Id, id)
                .With(t => t.Type, EventType.PenaltyShootout)
                .With(t => t.PeriodType, PeriodType.Penalties)
                .With(t => t.Team, isHome ? "home" : "away")
                .With(t => t.Player, new Player("player_id", playerName))
                .With(t => t.IsHomeShootoutScored, isHome && isScored)
                .With(t => t.IsAwayShootoutScored, !isHome && isScored);
    }
}