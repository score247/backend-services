using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IBus messageBus;
        private readonly ICacheManager cacheManager;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IRedCardEventMessage> context;

        private readonly RedCardEventConsumer consumer;

        public RedCardEventConsumerTests()
        {
            messageBus = Substitute.For<IBus>();
            cacheManager = Substitute.For<ICacheManager>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<IRedCardEventMessage>>();

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
                new MatchResult(),
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
                new MatchResult(),
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
            => new TimelineEvent { Type = EventType.RedCard };

        private static TimelineEvent StubYellowRedCard()
           => new TimelineEvent { Type = EventType.YellowRedCard };
    }
}
