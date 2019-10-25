using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Caching;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Matches.MatchEvents;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.MatchEvents
{
    [Trait("Soccer.EventProcessors", "PenaltyEventConsumer")]
    public class PenaltyEventConsumerTests
    {
        private readonly IBus messageBus;
        private readonly ICacheService cacheService;
        private readonly ConsumeContext<IPenaltyEventMessage> context;

        private readonly PenaltyEventConsumer consumer;

        public PenaltyEventConsumerTests()
        {
            messageBus = Substitute.For<IBus>();
            cacheService = Substitute.For<ICacheService>();
            context = Substitute.For<ConsumeContext<IPenaltyEventMessage>>();

            consumer = new PenaltyEventConsumer(messageBus, cacheService);
        }

        [Fact]
        public async Task Consume_MatchEventIsNull_ShouldNotPublishMatchEventProcessed()
        {
            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_CachedPenaltyShootout_ShouldNotPublishMatchEventProcessed()
        {
            var timeline = A.Dummy<TimelineEvent>().With(t => t.Id, "1");
            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                timeline
                )));
            cacheService.GetAsync<IList<TimelineEvent>>(Arg.Any<string>()).Returns(new List<TimelineEvent> {
                timeline
            });

            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_NotCachedPenaltyShootout_ShouldPublishMatchEventProcessed()
        {
            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>().With(t => t.Id, "1")
                )));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_FirstCachedPenaltyShootout_FirstShootIsTrue()
        {
            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>().With(t => t.Id, "1")
                )));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.IsFirstShoot));
        }

        [Fact]
        public async Task Consume_FirstCachedHomePenaltyShootout_AssignedHomeShootoutPlayer()
        {
            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                StubHomePenaltyShootout(true)
                )));

            cacheService.GetAsync<IList<TimelineEvent>>(Arg.Any<string>()).Returns(new List<TimelineEvent>());

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt =>
                    evt.MatchEvent.Timeline.IsFirstShoot &&
                    evt.MatchEvent.Timeline.HomeShootoutPlayer.Name == "playerName" &&
                    evt.MatchEvent.Timeline.ShootoutHomeScore == 1 &&
                    evt.MatchEvent.Timeline.ShootoutAwayScore == 0));
        }

        [Fact]
        public async Task Consume_FirstCachedAwayPenaltyShootout_AssignedAwayShootoutPlayer()
        {
            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                StubAwayPenaltyShootout(true)
                )));

            cacheService.GetAsync<IList<TimelineEvent>>(Arg.Any<string>()).Returns(new List<TimelineEvent>());

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt =>
                    evt.MatchEvent.Timeline.IsFirstShoot &&
                    evt.MatchEvent.Timeline.AwayShootoutPlayer.Name == "playerName" &&
                    evt.MatchEvent.Timeline.ShootoutAwayScore == 1 &&
                    evt.MatchEvent.Timeline.ShootoutHomeScore == 0));
        }

        [Fact]
        public async Task Handle_CountTotalScore_ReturnShootoutEventWithTotalScore()
        {
            var matchId = "CountTotalScore";
            var shootoutEvent = StubTimeline("5", isHome: true, isScored: true);
            cacheService.GetAsync<IList<TimelineEvent>>($"Penalty_Match_{matchId}").Returns(
                Task.FromResult(new List<TimelineEvent>
                {
                    StubTimeline("1", isHome: true, isScored: true),
                    StubTimeline("2",isHome: false, isScored: true),
                    StubTimeline("3",isHome: true, isScored: false),
                    StubTimeline("4",isHome: false, isScored: false)
                } as IList<TimelineEvent>));

            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
              "sr:league",
              matchId,
              A.Dummy<MatchResult>(),
              shootoutEvent
              )));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
               Arg.Is<MatchEventProcessedMessage>(evt =>
                   evt.MatchEvent.Timeline.ShootoutHomeScore == 2 &&
                   evt.MatchEvent.Timeline.ShootoutAwayScore == 1));
        }

        [Fact]
#pragma warning disable S2699 // Tests should include assertions
        public async Task Handle_Always_SetCacheForNewShootoutEvent()
#pragma warning restore S2699 // Tests should include assertions
        {
            var matchId = "SetCacheForNewShootoutEvent";
            var cacheKeyEvent = "Penalty_Match_SetCacheForNewShootoutEvent";
            var shootoutEvent = StubTimeline("1", true, true, "ricky");

            cacheService.GetAsync<IList<TimelineEvent>>(cacheKeyEvent).Returns(Task.FromResult(new List<TimelineEvent>() as IList<TimelineEvent>));

            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
               "sr:league",
               matchId,
               A.Dummy<MatchResult>(),
               shootoutEvent
               )));

            await consumer.Consume(context);

            await cacheService.Received().SetAsync(
                "Penalty_Match_SetCacheForNewShootoutEvent",
                Arg.Is<IList<TimelineEvent>>(
                    timelines => timelines.Count == 1
                        && timelines.First().HomeShootoutPlayer.Name == "ricky"
                        && timelines.First().IsHomeShootoutScored),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task Handle_LastEventExistsInCache_ReturnCombineShootEventAndRemoveCache()
        {
            var matchId = "CombineShootEvent";
            var latestEventCacheKey = $"Penalty_Match_{matchId}_Latest_Event";
            var shootoutEvent = StubTimeline("2", false, false, "awayPlayerName");
            var latestShootoutEvent = StubTimeline("1", true, true, "homePlayerName");
            latestShootoutEvent.HomeShootoutPlayer = latestShootoutEvent.Player;

            cacheService.GetAsync<TimelineEvent>(latestEventCacheKey)
                .Returns(Task.FromResult(latestShootoutEvent));

            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                shootoutEvent
                )));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt =>
                    evt.MatchEvent.Timeline.AwayShootoutPlayer.Name == "awayPlayerName" &&
                    !evt.MatchEvent.Timeline.IsAwayShootoutScored &&
                    evt.MatchEvent.Timeline.HomeShootoutPlayer.Name == "homePlayerName" &&
                    evt.MatchEvent.Timeline.IsHomeShootoutScored
                   ));

            await cacheService.Received().RemoveAsync(latestEventCacheKey);
        }

        [Fact]
        public async Task Handle_LastestAwayShootoutInCache_ReturnCombineShootEventAndRemoveCache()
        {
            var matchId = "CombineShootEvent";
            var latestEventCacheKey = $"Penalty_Match_{matchId}_Latest_Event";
            var shootoutEvent = StubTimeline("2", isHome: true, isScored: false, "homePlayerName");
            var latestShootoutEvent = StubTimeline("1", isHome: false, isScored: true, "awayPlayerName");
            latestShootoutEvent.AwayShootoutPlayer = latestShootoutEvent.Player;

            cacheService.GetAsync<TimelineEvent>(latestEventCacheKey)
                .Returns(Task.FromResult(latestShootoutEvent));

            context.Message.Returns(new PenaltyEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                shootoutEvent
                )));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt =>
                    evt.MatchEvent.Timeline.AwayShootoutPlayer.Name == "awayPlayerName" &&
                    evt.MatchEvent.Timeline.IsAwayShootoutScored &&
                    evt.MatchEvent.Timeline.HomeShootoutPlayer.Name == "homePlayerName" &&
                    !evt.MatchEvent.Timeline.IsHomeShootoutScored
                   ));

            await cacheService.Received().RemoveAsync(latestEventCacheKey);
        }

        private static TimelineEvent StubHomePenaltyShootout(bool scored)
            => StubTimeline("1", true, scored);

        private static TimelineEvent StubAwayPenaltyShootout(bool scored)
            => StubTimeline("1", false, scored);

        private static TimelineEvent StubTimeline(string id, bool isHome, bool isScored, string playerName = "playerName")
        {
            var timeline = A.Dummy<TimelineEvent>()
                  .With(t => t.Id, id)
                  .With(t => t.Team, isHome ? "home" : "away")
                  .With(t => t.Player, new Player("player_id", playerName))
                  ;

            timeline.IsHomeShootoutScored = isHome && isScored;
            timeline.IsAwayShootoutScored = !isHome && isScored;

            return timeline;
        }
    }
}