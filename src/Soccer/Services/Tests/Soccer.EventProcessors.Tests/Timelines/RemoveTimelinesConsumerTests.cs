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
using Soccer.EventProcessors.Timeline;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timelines
{
    public class RemoveTimelinesConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IMatchTimelinesConfirmedMessage> context;
        private readonly Fixture fixture;

        private readonly RemoveTimelinesConsumer removeTimelineConsumer;

        public RemoveTimelinesConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            fixture = new Fixture();

            context = Substitute.For<ConsumeContext<IMatchTimelinesConfirmedMessage>>();

            removeTimelineConsumer = new RemoveTimelinesConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_MessageIsNull_NotFetchTimelines()
        {
            // Arrange

            // Act
            await removeTimelineConsumer.Consume(context);

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
            await removeTimelineConsumer.Consume(context);

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
            await removeTimelineConsumer.Consume(context);

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
            await removeTimelineConsumer.Consume(context);

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
            await removeTimelineConsumer.Consume(context);

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
            await removeTimelineConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .DidNotReceive()
                .ExecuteAsync(Arg.Is<RemoveTimelineCommand>(command => command.MatchId == message.MatchId));
        }

        [Fact]
        public async Task Consume_HasRemovedTimelines_ShouldExecuteCommandWithCorrectTimelines()
        {
            // Arrange           
            var timelines = new List<TimelineEvent>
            {
                StubTimeline("1", new DateTime(2019, 10, 10, 10, 20, 00)),
                StubTimeline("3", new DateTime(2019, 10, 10, 10, 22, 00)),
            };

            var currentTimelines = new List<TimelineEvent>
            {
                StubTimeline("sr:timeline:2", new DateTime(2019, 10, 10, 10, 21, 00)),
                StubTimeline("sr:timeline:3", new DateTime(2019, 10, 10, 10, 21, 30))
            };

            currentTimelines.AddRange(timelines);

            var message = A.Dummy<MatchTimelinesConfirmedMessage>()
                .With(msg => msg.MatchId, fixture.Create<string>())
                .With(msg => msg.Timelines, timelines);

            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(criteria => criteria.MatchId == message.MatchId))
                .Returns(currentTimelines);

            // Act
            await removeTimelineConsumer.Consume(context);

            // Assert
            await dynamicRepository
                .Received(1)
                .ExecuteAsync(Arg.Is<RemoveTimelineCommand>(command => 
                    command.MatchId == message.MatchId &&
                    command.TimelineIds.Contains("sr:timeline:2") &&
                    command.TimelineIds.Contains("sr:timeline:3")
                ));
        }

        private TimelineEvent StubTimeline(string id, DateTimeOffset time)
            => fixture
                .Create<TimelineEvent>()
                .With(timeline => timeline.Id, id)
                .With(timeline => timeline.Time, time);
    }
}
