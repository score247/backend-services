using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Leagues.Services;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    public class FetchPostMatchesConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;
        private readonly ConsumeContext<IPostMatchFetchedMessage> context;

        private readonly FetchPostMatchesConsumer consumer;

        public FetchPostMatchesConsumerTests() 
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            leagueService = Substitute.For<ILeagueService>();
            context = Substitute.For<ConsumeContext<IPostMatchFetchedMessage>>();

            consumer = new FetchPostMatchesConsumer(dynamicRepository, leagueService);
        }

        [Fact]
        public async Task Consume_MatchesEmpty_NotGetMajorLeagues()
        {
            await consumer.Consume(context);

            await leagueService.DidNotReceive().GetMajorLeagues();
        }

        [Fact]
        public async Task Consume_HasMatches_AlwaysGetMajorLeagues()
        {
            var fixture = new Fixture();
            var matches = fixture.CreateMany<Match>(4);
            context.Message
                .Returns(A.Dummy<PostMatchFetchedMessage>()
                    .With(message => message.Matches, matches)
                    .With(message => message.Language, Language.en_US.DisplayName));

            await consumer.Consume(context);

            await leagueService.Received(1).GetMajorLeagues();
        }

        [Fact]
        public async Task Consume_HasMatches_ExecuteInsertOrUpdateMatchesCommand()
        {
            var fixture = new Fixture();
            var matches = fixture.CreateMany<Match>(4);
            context.Message
                .Returns(A.Dummy<PostMatchFetchedMessage>()
                    .With(message => message.Matches, matches)
                    .With(message => message.Language, Language.en_US.DisplayName));

            await consumer.Consume(context);

            await dynamicRepository.Received().ExecuteAsync(Arg.Any<InsertOrUpdatePostMatchesCommand>());
        }
    }
}
