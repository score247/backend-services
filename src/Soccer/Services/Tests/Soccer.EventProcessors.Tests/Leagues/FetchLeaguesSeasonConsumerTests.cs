using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;
using Soccer.EventProcessors.Leagues;
using Xunit;

namespace Soccer.EventProcessors.Tests.Leagues
{
    [Trait("Soccer.EventProcessors", "FetchLeaguesSeasonConsumer")]
    public class FetchLeaguesSeasonConsumerTests
    {
        private readonly ConsumeContext<ILeaguesSeasonFetchedMessage> context;
        private readonly IDynamicRepository dynamicRepository;
        private readonly FetchLeaguesSeasonConsumer consumer;

        public FetchLeaguesSeasonConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<ILeaguesSeasonFetchedMessage>>();

            consumer = new FetchLeaguesSeasonConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_LeaguesSeasonFetchedMessage_EmptyLeagues_NotExecuteAnyCommands()
        {
            context.Message.Returns(new LeaguesSeasonFetchedMessage(
                Enumerable.Empty<League>()
                ));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertLeagueSeasonCommand>());
        }

        [Fact]
        public async Task Consume_LeaguesSeasonFetchedMessage_ExecuteInsertLeagueSeasonCommand()
        {
            context.Message.Returns(new LeaguesSeasonFetchedMessage(
                A.CollectionOfDummy<League>(5)
                ));

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<InsertLeagueSeasonCommand>());
        }
    }
}
