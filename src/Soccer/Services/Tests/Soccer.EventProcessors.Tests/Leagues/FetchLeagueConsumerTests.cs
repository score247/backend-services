using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Leagues.Commands;
using Soccer.EventProcessors.Leagues;
using Xunit;

namespace Soccer.EventProcessors.Tests.Leagues
{
    [Trait("Soccer.EventProcessors", "FetchLeagueConsumer")]
    public class FetchLeagueConsumerTests
    {
        private readonly ConsumeContext<ILeaguesFetchedMessage> context;
        private readonly IDynamicRepository dynamicRepository;
        private readonly FetchLeaguesConsumer consumer;


        public FetchLeagueConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<ILeaguesFetchedMessage>>();

            consumer = new FetchLeaguesConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_LeaguesFetchedMessage_EmptyLeagues_NotExecuteInsertLeagueSeasonCommand()
        {
            context.Message.Returns(new LeaguesFetchedMessage(
                Enumerable.Empty<League>(),
                Language.en_US.DisplayName
                ));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertLeagueSeasonCommand>());
        }

        [Fact]
        public async Task Consume_LeaguesFetchedMessage_ExecuteInsertLeagueSeasonCommand()
        {
            context.Message.Returns(new LeaguesFetchedMessage(
                A.CollectionOfDummy<League>(5),
                Language.en_US.DisplayName
                ));

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<InsertLeagueSeasonCommand>());
        }
    }
}
