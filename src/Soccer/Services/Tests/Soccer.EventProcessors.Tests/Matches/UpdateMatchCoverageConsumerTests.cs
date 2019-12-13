using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    public class UpdateMatchCoverageConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IMatchUpdatedCoverageInfo> context;

        private readonly UpdateMatchCoverageConsumer consumer;

        public UpdateMatchCoverageConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<IMatchUpdatedCoverageInfo>>();

            consumer = new UpdateMatchCoverageConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_Always_ExecuteUpdateMatchCoverageCommand()
        {
            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateMatchCoverageCommand>());
        }
    }
}
