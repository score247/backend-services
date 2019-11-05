using System.Threading.Tasks;
using FakeItEasy;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors.Matches.MatchEvents;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.MatchEvents
{
    public class BreakStartEventConsumerTests
    {
        private readonly IBus messageBus;
        private readonly ConsumeContext<IBreakStartEventMessage> context;

        private readonly BreakStartEventConsumer breakStartConsumer;

        public BreakStartEventConsumerTests() 
        {
            context = Substitute.For<ConsumeContext<IBreakStartEventMessage>>();
            messageBus = Substitute.For<IBus>();

            breakStartConsumer = new BreakStartEventConsumer(messageBus);
        }

        [Fact]
        public async Task Consume_MatchEventNull_ShouldNotPublishProcessedEventMessage()
        {
            await breakStartConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_BreakStartAndPeriodPause_ShouldPublishProcessedEventMessage()
        {
            context.Message.Returns(new BreakStartEventMessage
                (
                    new MatchEvent(
                        "sr:league", 
                        "sr:match", 
                        A.Dummy<MatchResult>(), 
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.BreakStart)
                            .With(timeline => timeline.PeriodType, PeriodType.Pause))
                ));

            await breakStartConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 45));
        }

        [Fact]
        public async Task Consume_BreakStartAndAwaitingExtraTime_ShouldPublishProcessedEventMessage()
        {
            context.Message.Returns(new BreakStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.BreakStart)
                            .With(timeline => timeline.PeriodType, PeriodType.AwaitingExtraTime))
                ));

            await breakStartConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 90));
        }

        [Fact]
        public async Task Consume_BreakStartAndAwaitingPenalties_ShouldPublishProcessedEventMessage()
        {
            context.Message.Returns(new BreakStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.BreakStart)
                            .With(timeline => timeline.PeriodType, PeriodType.AwaitingPenalties))
                ));

            await breakStartConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 120));
        }

        [Fact]
        public async Task Consume_BreakStartAndPeriodNotValid_ShouldPublishProcessedEventMessage()
        {
            context.Message.Returns(new BreakStartEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        A.Dummy<MatchResult>(),
                        A.Dummy<TimelineEvent>()
                            .With(timeline => timeline.Type, EventType.BreakStart)
                            .With(timeline => timeline.PeriodType, PeriodType.ExtraTimeHalfTime))
                ));

            await breakStartConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 45));
        }
    }
}
