using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using NSubstitute;
using Score247.Shared;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Leagues.Filters;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    [Trait("Soccer.EventProcessors", "ReceiveMatchEventConsumer")]
    public class ReceiveMatchEventConsumerTests
    {
        private static readonly Fixture fixture = new Fixture();
        private readonly ICacheManager cacheManager;
        private readonly IBus messageBus;
        private readonly IMajorLeagueFilter<MatchEvent, bool> matchEventFilter;
        private readonly ReceiveMatchEventConsumer consumer;
        private readonly ConsumeContext<IMatchEventReceivedMessage> context;

        public ReceiveMatchEventConsumerTests()
        {
            cacheManager = Substitute.For<ICacheManager>();
            messageBus = Substitute.For<IBus>();
            matchEventFilter = Substitute.For<IMajorLeagueFilter<MatchEvent, bool>>();
            context = Substitute.For<ConsumeContext<IMatchEventReceivedMessage>>();

            var logger = Substitute.For<ILogger>();
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            consumer = new ReceiveMatchEventConsumer(cacheManager, dynamicRepository, messageBus, logger, matchEventFilter);
        }

        [Fact]
        public async Task Consume_MatchEventIsNull_ShouldNotPublishMatchEventProcessed()
        {
            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_NotInMajorLeague_ShouldNotMatchEventProcessed()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.Create<TimelineEvent>())));

            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ScoreChangeInPenalty_ShouldNotMatchEventProcessed()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Type, EventType.ScoreChange)
                    .With(t => t.PeriodType, PeriodType.Penalties)
                    .Create())));
            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ScoreChange_PublishMatchEventProcessed()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Type, EventType.ScoreChange).Create())));

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_PeriodStart_PublishPeriodStartEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Type, EventType.PeriodStart).Create())));

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IPeriodStartEventMessage>(Arg.Any<PeriodStartEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ShootOutInPenaltyAndNotProcessed_PublishPenaltyEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                StubPenaltyShootout())));

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IPenaltyEventMessage>(Arg.Any<PenaltyEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ShootOutInPenaltyAndProcessed_NotPublishPenaltyEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                StubPenaltyShootout())));

            cacheManager.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<IList<TimelineEvent>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<TimelineEvent> { StubPenaltyShootout() });

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IPenaltyEventMessage>(Arg.Any<PenaltyEventMessage>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_MatchEnd_PublishPeriodStartEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Type, EventType.MatchEnded).Create())));

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEndEventMessage>(Arg.Any<MatchEndEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_RedCard_PublishRedCardEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Type, EventType.RedCard).Create())));

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IRedCardEventMessage>(Arg.Any<RedCardEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ExistingRedCard_UpdateAndPublishRedCardEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Id, "1")
                    .With(t => t.Type, EventType.RedCard)
                    .With(t => t.Player, new Player("", "player"))
                    .Create())));

            cacheManager.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<IList<TimelineEvent>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<TimelineEvent> { fixture.For<TimelineEvent>()
                    .With(t => t.Id, "1")
                    .With(t => t.Type, EventType.RedCard)
                    .Create() });

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IRedCardEventMessage>(Arg.Is<RedCardEventMessage>(m => m.MatchEvent.Timeline.Player.Name == "player"));
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_YellowRedCard_PublishRedCardEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                fixture.Create<MatchResult>(),
                fixture.For<TimelineEvent>()
                    .With(t => t.Type, EventType.YellowRedCard)
                    .Create())));

            StubForMajorLeague("sr:league");

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IRedCardEventMessage>(Arg.Any<RedCardEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        private void StubForMajorLeague(string leagueId)
        {
            matchEventFilter.Filter(Arg.Is<MatchEvent>(evt => evt.LeagueId == leagueId)).Returns(true);
        }

        private static TimelineEvent StubPenaltyShootout()
            => fixture.For<TimelineEvent>()
                .With(t => t.Type, EventType.PenaltyShootout)
                .With(t => t.PeriodType, PeriodType.Penalties)
                .Create();
    }
}