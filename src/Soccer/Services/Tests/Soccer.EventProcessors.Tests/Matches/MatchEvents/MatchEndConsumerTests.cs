using System.Collections.Generic;
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
    public class MatchEndConsumerTests
    {
        private readonly IBus messageBus;
        private readonly ConsumeContext<IMatchEndEventMessage> context;

        private readonly MatchEndEventConsumer matchEndConsumer;

        public MatchEndConsumerTests()
        {
            context = Substitute.For<ConsumeContext<IMatchEndEventMessage>>();
            messageBus = Substitute.For<IBus>();

            matchEndConsumer = new MatchEndEventConsumer(messageBus);
        }

        [Fact]
        public async Task Consume_MatchEventNull_ShouldNotPublishProcessedEventMessage()
        {
            await matchEndConsumer.Consume(context);

            await messageBus.DidNotReceive().Publish<IMatchEventProcessedMessage>(Arg.Any<MatchEventProcessedMessage>());
        }

        [Fact]
        public async Task Consume_EndMatchButEmptyPeriods_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new MatchEndEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        StubEmptyMatchPeriodsResult(),
                        StubMatchEnd())
                ));

            await matchEndConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 91));
        }

        [Fact]
        public async Task Consume_EndNormalMatch_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new MatchEndEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        StubNormalMatch(),
                        StubMatchEnd())
                ));

            await matchEndConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 91));
        }

        [Fact]
        public async Task Consume_EndMatchHasExtraTime_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new MatchEndEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        StubMatchHasExtraTime(),
                        StubMatchEnd())
                ));

            await matchEndConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 121));
        }

        [Fact]
        public async Task Consume_EndMatchHasPenalty_ShouldNotPublishProcessedEventMessage()
        {
            context.Message.Returns(new MatchEndEventMessage
                (
                    new MatchEvent(
                        "sr:league",
                        "sr:match",
                        StubMatchHasPenalties(),
                        StubMatchEnd())
                ));

            await matchEndConsumer.Consume(context);

            await messageBus.Received(1).Publish<IMatchEventProcessedMessage>(
                Arg.Is<MatchEventProcessedMessage>(evt => evt.MatchEvent.Timeline.MatchTime == 122));
        }

        private static TimelineEvent StubMatchEnd()
            => A.Dummy<TimelineEvent>().With(timeline => timeline.Type, EventType.MatchEnded);

        private static MatchResult StubNormalMatch()
            => A.Dummy<MatchResult>().With(result => result.MatchPeriods, new List<MatchPeriod>
            {
                StubRegularPeriod(),
                StubRegularPeriod()
            });

        private static MatchResult StubMatchHasExtraTime()
          => A.Dummy<MatchResult>().With(result => result.MatchPeriods, new List<MatchPeriod>
          {
                StubRegularPeriod(),
                StubRegularPeriod(),
                StubOvertimePeriod()
          });

        private static MatchResult StubMatchHasPenalties()
          => A.Dummy<MatchResult>().With(result => result.MatchPeriods, new List<MatchPeriod>
          {
                StubRegularPeriod(),
                StubRegularPeriod(),
                StubOvertimePeriod(),
                StubPenaltiesPeriod()
          });

        private static MatchResult StubEmptyMatchPeriodsResult()
            => A.Dummy<MatchResult>().With(result => result.MatchPeriods, new List<MatchPeriod>());

        private static MatchPeriod StubRegularPeriod()
            => A.Dummy<MatchPeriod>().With(period => period.PeriodType, PeriodType.RegularPeriod);

        private static MatchPeriod StubOvertimePeriod()
            => A.Dummy<MatchPeriod>().With(period => period.PeriodType, PeriodType.Overtime);

        private static MatchPeriod StubPenaltiesPeriod()
           => A.Dummy<MatchPeriod>().With(period => period.PeriodType, PeriodType.Penalties);
    }
}
