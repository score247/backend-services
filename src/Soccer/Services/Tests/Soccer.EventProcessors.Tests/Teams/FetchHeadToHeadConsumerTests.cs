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
using Soccer.Core.Teams.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Services;
using Soccer.EventProcessors.Shared.Configurations;
using Soccer.EventProcessors.Teams;
using Xunit;

namespace Soccer.EventProcessors.Tests.Teams
{
    public class FetchHeadToHeadConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;
        private readonly IAppSettings appSettings;
        private readonly ConsumeContext<IHeadToHeadFetchedMessage> context;
        private readonly Fixture fixture;

        private readonly FetchHeadToHeadConsumer consumer;

        public FetchHeadToHeadConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            leagueService = Substitute.For<ILeagueService>();
            appSettings = Substitute.For<IAppSettings>();
            context = Substitute.For<ConsumeContext<IHeadToHeadFetchedMessage>>();

            fixture = new Fixture();
            appSettings.HeadToHeadIntervalInYears.Returns((byte)1);
            consumer = new FetchHeadToHeadConsumer(dynamicRepository, leagueService, appSettings);
        }

        [Fact]
        public async Task Consume_MatchNull_NotExecuteInsertCommand()
        {
            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertOrUpdateHeadToHeadCommand>());
        }

        [Fact]
        public async Task Consume_EventDateBeforeIntervalYear_NotExecuteInsertCommand()
        {
            var current = DateTime.Now;
            var matchInYearBefore = A.Dummy<Match>()
                .With(match => match.EventDate, current.AddYears(-appSettings.HeadToHeadIntervalInYears).AddMinutes(-1));

            context.Message.Returns(A.Dummy<HeadToHeadFetchedMessage>().With(msg => msg.Match, matchInYearBefore));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertOrUpdateHeadToHeadCommand>());
        }

        [Theory]
        [InlineData(11)]
        [InlineData(6)]
        [InlineData(0)]
        public async Task Consume_MatchInIntervalYear_ExecuteInsertCommand(int months)
        {
            var current = DateTime.Now;
            var match = A.Dummy<Match>()
                .With(match => match.EventDate, current.AddMonths(-months))
                .With(match => match.Teams, StubTeams());

            context.Message.Returns(A.Dummy<HeadToHeadFetchedMessage>().With(msg => msg.Match, match));

            await consumer.Consume(context);

            await dynamicRepository
                .Received(1)
                .ExecuteAsync(Arg.Is<InsertOrUpdateHeadToHeadCommand>(command =>
                    command.HomeTeamId == match.Teams.First(t => t.IsHome).Id &&
                    command.AwayTeamId == match.Teams.First(t => !t.IsHome).Id));
        }

        private List<Team> StubTeams()
        => new List<Team>
            {
                fixture.Create<Team>().With(team => team.IsHome, true),
                fixture.Create<Team>().With(team => team.IsHome, false)
            };
    }
}
