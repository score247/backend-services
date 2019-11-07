using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Matches.MatchEvents;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.MatchEvents
{
    public class PeriodStartEventConsumerTests
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IPeriodStartEventMessage> context;

        private readonly PeriodStartEventConsumer periodStartConsumer;

        public PeriodStartEventConsumerTests()
        {
            context = Substitute.For<ConsumeContext<IPeriodStartEventMessage>>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            messageBus = Substitute.For<IBus>();

            periodStartConsumer = new PeriodStartEventConsumer(messageBus, dynamicRepository);
        }

        [Fact]
        public async Task Consume_MatchEventNull_ShouldNotPublishProcessedEventMessage()
        {
            await periodStartConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_FirstRegularPeriodStart_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new PeriodStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.PeriodStart)
                            .With(timeline => timeline.PeriodType, PeriodType.RegularPeriod)
                            .With(timeline => timeline.Period, 1))
                ));

            await periodStartConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateLiveMatchCurrentPeriodStartTimeCommand>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 1));
        }

        [Fact]
        public async Task Consume_SecondRegularPeriodStart_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new PeriodStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.PeriodStart)
                            .With(timeline => timeline.PeriodType, PeriodType.RegularPeriod)
                            .With(timeline => timeline.Period, 2))
                ));

            await periodStartConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateLiveMatchCurrentPeriodStartTimeCommand>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 46));
        }

        [Fact]
        public async Task Consume_FirstExtraPeriodStart_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new PeriodStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.PeriodStart)
                            .With(timeline => timeline.PeriodType, PeriodType.Overtime)
                            .With(timeline => timeline.Period, 1))
                ));

            await periodStartConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateLiveMatchCurrentPeriodStartTimeCommand>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 91));
        }

        [Fact]
        public async Task Consume_SecondExtraPeriodStart_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new PeriodStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.PeriodStart)
                            .With(timeline => timeline.PeriodType, PeriodType.Overtime)
                            .With(timeline => timeline.Period, 2))
                ));

            await periodStartConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateLiveMatchCurrentPeriodStartTimeCommand>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 106));
        }

        [Fact]
        public async Task Consume_PenaltiesPeriodStart_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new PeriodStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.PeriodStart)
                            .With(timeline => timeline.PeriodType, PeriodType.Penalties))
                ));

            await periodStartConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateLiveMatchCurrentPeriodStartTimeCommand>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 121));
        }

        [Fact]
        public async Task Consume_EmptyPeriodType_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new PeriodStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.PeriodStart)
                            .With(timeline => timeline.PeriodType, new PeriodType()))
                ));

            await periodStartConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateLiveMatchCurrentPeriodStartTimeCommand>());
            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 1));
        }
    }
}
