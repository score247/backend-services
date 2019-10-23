using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.Caching;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Score247.Shared;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.QueueMessages;
using Soccer.EventProcessors.Matches.MatchEvents;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.MatchEvents
{
    [Trait("Soccer.EventProcessors", "RedCardEventConsumer")]
    public class RedCardEventConsumerTests
    {
        private static readonly Fixture fixture = new Fixture();
        private readonly IBus messageBus;
        private readonly ICacheManager cacheManager;
        private readonly ConsumeContext<IRedCardEventMessage> context;
        private readonly RedCardEventConsumer consumer;

        public RedCardEventConsumerTests()
        {
            messageBus = Substitute.For<IBus>();
            cacheManager = Substitute.For<ICacheManager>();
            context = Substitute.For<ConsumeContext<IRedCardEventMessage>>();

            var dynamicRepository = Substitute.For<IDynamicRepository>();
            consumer = new RedCardEventConsumer(messageBus, cacheManager, dynamicRepository);
        }

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotPublishMatchEvent()
        {
            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task Consume_RedCard_ShouldPublishCorrectStatistic()
        {
            var matchId = "sr:match";
            context.Message.Returns(new RedCardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                fixture.Create<MatchResult>(),
                StubRedCard()
                )));

            cacheManager.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<IList<TimelineEvent>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<TimelineEvent>
                {
                    StubRedCard()
                });

            await consumer.Consume(context);

            await messageBus.Received(1).Publish(Arg.Any<MatchEventProcessedMessage>());
            await messageBus.Received(1).Publish(Arg.Is<TeamStatisticUpdatedMessage>(stats => stats.TeamStatistic.RedCards == 1));
        }

        [Fact]
        public async Task Consume_RedCardAndYellowRedCard_ShouldPublishCorrectStatistic()
        {
            var matchId = "sr:match";
            context.Message.Returns(new RedCardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                fixture.Create<MatchResult>(),
                StubRedCard()
                )));

            cacheManager.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<IList<TimelineEvent>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<TimelineEvent>
                {
                    StubRedCard(),
                    StubYellowRedCard()
                });

            await consumer.Consume(context);

            await messageBus.Received(1).Publish(Arg.Any<MatchEventProcessedMessage>());
            await messageBus.Received(1).Publish(Arg.Is<TeamStatisticUpdatedMessage>(
                stats => stats.TeamStatistic.RedCards == 1
                        && stats.TeamStatistic.YellowRedCards == 1));
        }

        private static TimelineEvent StubRedCard()
            => fixture.Build<TimelineEvent>()
                .With(t => t.Type, EventType.RedCard)
                .Create();

        private static TimelineEvent StubYellowRedCard()
            => fixture.Build<TimelineEvent>()
                .With(t => t.Type, EventType.YellowCard)
                .Create();
    }
}