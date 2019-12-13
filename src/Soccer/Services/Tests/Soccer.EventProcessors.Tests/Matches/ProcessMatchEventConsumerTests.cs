using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    public class ProcessMatchEventConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IMatchEventProcessedMessage> context;

        private readonly ProcessMatchEventConsumer consumer;

        public ProcessMatchEventConsumerTests() 
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<IMatchEventProcessedMessage>>();

            consumer = new ProcessMatchEventConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_MessageIsNull_NotExecuteAnyCommand() 
        {
            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertTimelineCommand>());
            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateLiveMatchResultCommand>());
            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateLiveMatchLastTimelineCommand>());
        }

        [Fact]
        public async Task Consume_MatchEventIsNull_NotExecuteAnyCommand()
        {
            context.Message.Returns(new MatchEventProcessedMessage(null));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertTimelineCommand>());
            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateLiveMatchResultCommand>());
            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateLiveMatchLastTimelineCommand>());
        }

        [Fact]
        public async Task Consume_MatchEventNotLatest_InsertTimelineOnly()
        {
            var matchEventMessage = A.Dummy<MatchEventProcessedMessage>();
            context.Message.Returns(matchEventMessage);

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertTimelineCommand>(timeline => timeline.MatchId == matchEventMessage.MatchEvent.MatchId ));
            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateLiveMatchResultCommand>());
            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateLiveMatchLastTimelineCommand>());
        }

        [Fact]
        public async Task Consume_LatestMatchEvent_UpdateLiveMatchResultAndTimeline()
        {
            var matchEvent = A.Dummy<MatchEvent>().With(matchEvent => matchEvent.IsLatest, true);
            var matchEventMessage = A.Dummy<MatchEventProcessedMessage>().With(message => message.MatchEvent, matchEvent);
            context.Message.Returns(matchEventMessage);

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertTimelineCommand>(cmd => cmd.MatchId == matchEvent.MatchId));
            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<UpdateLiveMatchResultCommand>(cmd => cmd.MatchId == matchEvent.MatchId));
            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<UpdateLiveMatchLastTimelineCommand>(cmd => cmd.MatchId == matchEvent.MatchId));
        }
    }
}
