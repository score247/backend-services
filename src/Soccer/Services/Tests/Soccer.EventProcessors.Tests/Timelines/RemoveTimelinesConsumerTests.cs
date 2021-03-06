using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.Database.Matches.Criteria;
using Soccer.Database.Timelines.Commands;
using Soccer.EventProcessors.Shared.Configurations;
using Soccer.EventProcessors.Timeline;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timelines
{
    public class RemoveTimelinesConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;
        private readonly ConsumeContext<IMatchTimelinesConfirmedMessage> context;
        private readonly Fixture fixture;

        private readonly RemoveTimelineEventsConsumer removeTimelineEventConsumer;

        public RemoveTimelinesConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            messageBus = Substitute.For<IBus>();
            appSettings = Substitute.For<IAppSettings>();
            fixture = new Fixture();

            context = Substitute.For<ConsumeContext<IMatchTimelinesConfirmedMessage>>();

            removeTimelineEventConsumer = new RemoveTimelineEventsConsumer(dynamicRepository, messageBus, appSettings);
        }

        [Fact]
        public async Task Consume_MessageIsNull_NotFetchTimelines()
        {
            // Arrange

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository.DidNotReceive().FetchAsync<TimelineEvent>(Arg.Any<GetTimelineEventsCriteria>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Consume_MatchIdNullOrWhiteSpace_NotFetchTimelines(string matchId)
        {
            // Arrange
            context.Message
                .Returns(A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, matchId));

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository.DidNotReceive().FetchAsync<TimelineEvent>(Arg.Any<GetTimelineEventsCriteria>());
        }

        [Fact]
        public async Task Consume_EmptyTimelines_NotFetchTimelines()
        {
            // Arrange
            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, new List<TimelineEvent>());

            context.Message.Returns(message);

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository.DidNotReceive().FetchAsync<TimelineEvent>(Arg.Any<GetTimelineEventsCriteria>());
        }


        [Fact]
        public async Task Consume_CurrentTimelinesEmpty_ShouldNotExecuteCommand()
        {
            // Arrange           
            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, A.CollectionOfDummy<TimelineEvent>(5));

            context.Message.Returns(message);

            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(criteria => criteria.MatchId == message.MatchId))
                .Returns(new List<TimelineEvent>());

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .DidNotReceive()
                .ExecuteAsync(Arg.Is<RemoveTimelineCommand>(command => command.MatchId == message.MatchId));
        }

        [Fact]
        public async Task Consume_HasConfirmedTimelines_ShouldFetchTimelines()
        {
            // Arrange           
            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, A.CollectionOfDummy<TimelineEvent>(5));

            context.Message.Returns(message);

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .Received(1)
                .FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(criteria => criteria.MatchId == message.MatchId));
        }

        [Fact]
        public async Task Consume_NotHaveRemovedTimelines_ShouldNotExecuteCommand()
        {
            // Arrange           
            var timelines = new List<TimelineEvent>
            {
                StubTimeline("1", DateTimeOffset.Now)
            };

            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, timelines);

            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(criteria => criteria.MatchId == message.MatchId))
                .Returns(timelines);

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .DidNotReceive()
                .ExecuteAsync(Arg.Is<RemoveTimelineCommand>(command => command.MatchId == message.MatchId));
        }

        [Fact]
        public async Task Consume_HasRemovedTimelines_ShouldExecuteCommandWithCorrectTimelines()
        {
            // Arrange           
            var confirmedTimelines = new List<TimelineEvent>
            {
                StubTimeline("1", new DateTime(2019, 10, 10, 10, 20, 00)),
                StubTimeline("3", new DateTime(2019, 10, 10, 10, 22, 00)),
            };

            var incorrectTimelines = new List<TimelineEvent>
            {
                StubTimeline("sr:timeline:2", new DateTime(2019, 10, 10, 10, 21, 00)),
                StubTimeline("sr:timeline:3", new DateTime(2019, 10, 10, 10, 21, 30))
            };

            List<TimelineEvent> currentTimelines = StubCurrentTimelines(confirmedTimelines, incorrectTimelines);

            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, confirmedTimelines);

            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(criteria => criteria.MatchId == message.MatchId))
                .Returns(currentTimelines);

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .Received(1)
                .ExecuteAsync(Arg.Is<RemoveTimelineCommand>(command =>
                    command.MatchId == message.MatchId &&
                    command.TimelineIds.Contains("sr:timeline:2") &&
                    command.TimelineIds.Contains("sr:timeline:3")
                ));

            await messageBus
                .Received(1)
                .Publish(Arg.Is<MatchTimelinesRemovedMessage>(msg =>
                    msg.MatchId == message.MatchId &&
                    msg.TimelineIds.Count() == 2
                ));
        }

        [Fact]
        public async Task Consume_LiveMatch_CurrentTimelineInCorrectSpan_ShouldNotRemove()
        {
            // Arrange             
            appSettings.CorrectTimelineSpanInMinutes.Returns(1);

            var confirmedTimelines = new List<TimelineEvent>
            {
                StubTimeline("1", DateTime.Now.AddMinutes(-10)),
                StubTimeline("3", DateTime.Now.AddMinutes(-9)),
            };

            var incorrectTimelines = new List<TimelineEvent>
            {
                StubTimeline("sr:timeline:2", DateTime.Now.AddMinutes(-4)),
                StubTimeline("sr:timeline:3", DateTime.Now)
            };

            List<TimelineEvent> currentTimelines = StubCurrentTimelines(confirmedTimelines, incorrectTimelines);

            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, confirmedTimelines);

            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(criteria => criteria.MatchId == message.MatchId))
                .Returns(currentTimelines);

            // Act
            await removeTimelineEventConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .Received(1)
                .ExecuteAsync(Arg.Is<RemoveTimelineCommand>(command =>
                    command.MatchId == message.MatchId &&
                    command.TimelineIds.Contains("sr:timeline:2")
                ));

            await messageBus
                .Received(1)
                .Publish(Arg.Is<MatchTimelinesRemovedMessage>(msg =>
                    msg.MatchId == message.MatchId &&
                    msg.TimelineIds.Count() == 1
                ));
        }

        private List<TimelineEvent> StubCurrentTimelines(List<TimelineEvent> confirmedTimelines, List<TimelineEvent> incorrectTimelines)
        {
            var currentTimelines = new List<TimelineEvent>(incorrectTimelines);

            currentTimelines.AddRange(confirmedTimelines);

            return currentTimelines;
        }

        private TimelineEvent StubTimeline(string id, DateTimeOffset time)
            => fixture
                .Create<TimelineEvent>()
                .With(timeline => timeline.Id, id)
                .With(timeline => timeline.Time, time);
    }
}
