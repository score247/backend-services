using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
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
        private readonly ConsumeContext<ICardEventMessage> context;
        private readonly CardEventConsumer consumer;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Fixture fixture;

        public RedCardEventConsumerTests()
        {
            fixture = new Fixture();
            messageBus = Substitute.For<IBus>();
            context = Substitute.For<ConsumeContext<ICardEventMessage>>();

            dynamicRepository = Substitute.For<IDynamicRepository>();
            consumer = new CardEventConsumer(messageBus, dynamicRepository);
        }

#pragma warning disable S2699 // Tests should include assertions

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotPublishMatchEvent()
        {
            await consumer.Consume(context);

            await messageBus.DidNotReceive().Publish(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task Consume_RedCard_ExistInDatabase_ShouldPublishCorrectStatistic()
        {
            const string matchId = "sr:match";
            context.Message.Returns(new CardEventMessage(new MatchEvent(
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
            await messageBus.Received(1).Publish(Arg.Is<TeamStatisticUpdatedMessage>(stats => stats.TeamStatistic.RedCards == 2));
        }

        [Fact]
        public async Task Consume_RedCard_NotExistInDatabase_ShouldPublishCorrectStatistic()
        {
            const string matchId = "sr:match";
            context.Message.Returns(new CardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                StubRedCard()
                )));

            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(c => c.MatchId == matchId))
                .Returns(new List<TimelineEvent>());

            await consumer.Consume(context);

            await messageBus.Received(1).Publish(Arg.Any<MatchEventProcessedMessage>());
            await messageBus.Received(1).Publish(Arg.Is<TeamStatisticUpdatedMessage>(stats => stats.TeamStatistic.RedCards == 1));
        }

        [Fact]
        public async Task Consume_YellowRedCard_NotExistInDatabase_ShouldPublishCorrectStatistic()
        {
            const string matchId = "sr:match";
            context.Message.Returns(new CardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                StubYellowRedCard()
                )));

            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(c => c.MatchId == matchId))
              .Returns(new List<TimelineEvent>
              {
                    StubRedCard()
              });

            await consumer.Consume(context);

            await messageBus.Received(1).Publish(Arg.Any<MatchEventProcessedMessage>());
            await messageBus.Received(1).Publish(Arg.Is<TeamStatisticUpdatedMessage>(
                stats => stats.TeamStatistic.RedCards == 1
                        && stats.TeamStatistic.YellowRedCards == 1));
        }

        [Fact]
        public async Task Consume_YellowRedCard_ExistInDatabase_ShouldPublishCorrectStatistic()
        {
            const string matchId = "sr:match";
            context.Message.Returns(new CardEventMessage(new MatchEvent(
                "sr:league",
                matchId,
                A.Dummy<MatchResult>(),
                StubYellowRedCard()
                )));

            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(c => c.MatchId == matchId))
              .Returns(new List<TimelineEvent> {
                  StubRedCard(),
                  StubYellowRedCard()
              });

            await consumer.Consume(context);

            await messageBus.Received(1).Publish(Arg.Any<MatchEventProcessedMessage>());
            await messageBus.Received(1).Publish(Arg.Is<TeamStatisticUpdatedMessage>(
                stats => stats.TeamStatistic.RedCards == 1
                        && stats.TeamStatistic.YellowRedCards == 2));
        }

        private TimelineEvent StubRedCard()
            => A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.RedCard)
                .With(t => t.Id, fixture.Create<string>());

        private TimelineEvent StubYellowRedCard()
            => A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.YellowRedCard)
                .With(t => t.Id, fixture.Create<string>());
    }

#pragma warning restore S2699 // Tests should include assertions
}