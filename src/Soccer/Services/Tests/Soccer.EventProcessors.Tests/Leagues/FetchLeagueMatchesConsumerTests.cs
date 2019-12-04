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
    [Trait("Soccer.EventProcessors", "FetchLeagueMatchesConsumer")]
    public class FetchLeagueMatchesConsumerTests
    {
        private readonly ConsumeContext<ILeagueMatchesFetchedMessage> context;
        private readonly IDynamicRepository dynamicRepository;

        private readonly FetchedLeagueMatchesConsumer consumer;

        public FetchLeagueMatchesConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<ILeagueMatchesFetchedMessage>>();

            consumer = new FetchedLeagueMatchesConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_LeaguesSeasonFetchedMessage_EmptyLeagueSeason_NotExecuteAnyCommands()
        {
            context.Message.Returns(new LeagueMatchesFetchedMessage(
                Enumerable.Empty<LeagueSeasonProcessedInfo>()
                ));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<UpdateFetchedLeagueSeasonCommand>());
        }

        [Fact]
        public async Task Consume_LeaguesSeasonFetchedMessage_ExecuteUpdateFetchedLeagueSeasonCommand()
        {
            context.Message.Returns(new LeagueMatchesFetchedMessage(
                A.CollectionOfDummy<LeagueSeasonProcessedInfo>(5)
                ));

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<UpdateFetchedLeagueSeasonCommand>());
        }
    }
}
