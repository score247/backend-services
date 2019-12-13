using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    public class UpdateMatchConditionsConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IMatchUpdatedConditionsMessage> context;

        private readonly UpdateMatchConditionsConsumer consumer;

        public UpdateMatchConditionsConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<IMatchUpdatedConditionsMessage>>();

            consumer = new UpdateMatchConditionsConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_MessageNull_NotExecuteUpdateMatchCoverageCommand()
        {
            context.Message.Returns(new MatchUpdatedConditionsMessage(null, null, 0, null, default));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateMatchRefereeAndAttendanceCommand>());
        }

        [Fact]
        public async Task Consume_MessageNotNull_ExecuteUpdateMatchCoverageCommand()
        {
            context.Message.Returns(A.Dummy<MatchUpdatedConditionsMessage>());

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateMatchRefereeAndAttendanceCommand>());
        }
    }
}
