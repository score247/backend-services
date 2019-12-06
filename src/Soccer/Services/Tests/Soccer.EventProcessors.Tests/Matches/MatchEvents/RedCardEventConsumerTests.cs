using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Caching;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Score247.Shared;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Matches.Criteria;
using Soccer.EventProcessors.Matches.MatchEvents;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.MatchEvents
{
    [Trait("Soccer.EventProcessors", "RedCardEventConsumer")]
    public class RedCardEventConsumerTests
    {
        private readonly IBus messageBus;
        private readonly ConsumeContext<IRedCardEventMessage> context;
        private readonly RedCardEventConsumer consumer;
        private readonly IDynamicRepository dynamicRepository;

        public RedCardEventConsumerTests()
        {
            messageBus = Substitute.For<IBus>();
            context = Substitute.For<ConsumeContext<IRedCardEventMessage>>();

            dynamicRepository = Substitute.For<IDynamicRepository>();
            consumer = new RedCardEventConsumer(messageBus, dynamicRepository);
        }

#pragma warning disable S2699 // Tests should include assertions

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotPublishMatchEvent()
        {
            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task Consume_RedCard_ShouldPublishCorrectStatistic()
        {
            const string matchId = "sr:match";
            context.Message.Returns(new RedCardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                StubRedCard()
                )));

            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(c => c.MatchId == matchId))
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
            const string matchId = "sr:match";
            context.Message.Returns(new RedCardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                StubRedCard()
                )));

            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(c => c.MatchId == matchId))
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
            => A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.RedCard);

        private static TimelineEvent StubYellowRedCard()
            => A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.YellowRedCard);
    }

#pragma warning restore S2699 // Tests should include assertions
}