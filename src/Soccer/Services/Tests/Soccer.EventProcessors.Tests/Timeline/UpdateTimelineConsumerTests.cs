using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timeline.QueueMessages;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Timelines;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timeline
{
    [Trait("Soccer.EventProcessors", "UpdateTimelineConsumer")]
    public class UpdateTimelineConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<ITimelineUpdatedMessage> context;
        private readonly UpdateTimelineConsumer updateTimelineConsumer;

        public UpdateTimelineConsumerTests() 
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<ITimelineUpdatedMessage>>();

            updateTimelineConsumer = new UpdateTimelineConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_MessageIsNull_ShouldNotExecuteCommand()
        {
            await updateTimelineConsumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertTimelineCommand>());
        }

        [Fact]
        public async Task Consume_MessageIsNull_ShouldExecuteCommand()
        {
            context.Message.Returns(new TimelineUpdatedMessage("match:1", "league:1", Language.en_US, new TimelineEvent()));

            await updateTimelineConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<InsertTimelineCommand>());
        }
    }
}
