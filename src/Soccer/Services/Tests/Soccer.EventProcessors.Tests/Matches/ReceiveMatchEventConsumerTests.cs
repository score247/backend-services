using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
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
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
#pragma warning disable S2699 // Tests should include assertions

    [Trait("Soccer.EventProcessors", "ReceiveMatchEventConsumer")]
    public class ReceiveMatchEventConsumerTests
    {
        private readonly ICacheManager cacheManager;
        private readonly IBus messageBus;
        private readonly ReceiveMatchEventConsumer consumer;
        private readonly ConsumeContext<IMatchEventReceivedMessage> context;

        public ReceiveMatchEventConsumerTests()
        {
            cacheManager = Substitute.For<ICacheManager>();
            messageBus = Substitute.For<IBus>();
            context = Substitute.For<ConsumeContext<IMatchEventReceivedMessage>>();

            var logger = Substitute.For<ILogger>();
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            consumer = new ReceiveMatchEventConsumer(cacheManager, dynamicRepository, messageBus, logger);
        }

        [Fact]
        public async Task Consume_MatchEventIsNull_ShouldNotPublishMatchEventProcessed()
        {
            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_LeaguesFetchedMessage_ShouldNotMatchEventProcessed()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.ScoreChange)
                    .With(t => t.PeriodType, PeriodType.Penalties)
                    )));

            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ScoreChange_PublishMatchEventProcessed()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.ScoreChange))));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_PeriodStart_PublishPeriodStartEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.PeriodStart))));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IPeriodStartEventMessage>(Arg.Any<PeriodStartEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_BreakStart_PublishBreakStartEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.BreakStart))));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IBreakStartEventMessage>(Arg.Any<BreakStartEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_ShootOutInPenaltyAndNotProcessed_PublishPenaltyEventMessage()
        {
            context.Message.Returns(new MatchEventReceivedMessage(new MatchEvent(
                "sr:league",
                "sr:match",
                A.Dummy<MatchResult>(),
                StubPenaltyShootout())));

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
                A.Dummy<MatchResult>(),
                StubPenaltyShootout())));

            cacheManager.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<IList<TimelineEvent>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<TimelineEvent> { StubPenaltyShootout() });

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
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.MatchEnded))));

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
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.RedCard))));

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
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Id, "1")
                    .With(t => t.Type, EventType.RedCard)
                    .With(t => t.Player, new Player("", "player"))
                    )));

            cacheManager.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<IList<TimelineEvent>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<TimelineEvent> { A.Dummy<TimelineEvent>()
                    .With(t => t.Id, "1")
                    .With(t => t.Type, EventType.RedCard)
                     });

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
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()
                    .With(t => t.Type, EventType.YellowRedCard)
                    )));

            await consumer.Consume(context);

            await messageBus.Received(1).Publish<IRedCardEventMessage>(Arg.Any<RedCardEventMessage>());
            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        private static TimelineEvent StubPenaltyShootout()
            => A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.PenaltyShootout)
                .With(t => t.PeriodType, PeriodType.Penalties);
    }

#pragma warning restore S2699 // Tests should include assertions
}