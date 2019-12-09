using System;
using System.Threading.Tasks;
using FakeItEasy;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Leagues
{
    [Trait("Soccer.DataReceivers.ScheduleTasks", "FetchLeagueSeasonTask")]
    public class FetchLeagueSeasonTaskTests
    {
        private readonly Func<DataProviderType, ILeagueService> leagueServiceFactory;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;
        private readonly ILeagueService leagueService;

        private readonly IFetchLeaguesSeasonTask fetchLeaguesSeason;

        public FetchLeagueSeasonTaskTests() 
        {
            appSettings = Substitute.For<IAppSettings>();
            messageBus = Substitute.For<IBus>();
            leagueService = Substitute.For<ILeagueService>();
            leagueServiceFactory = Substitute.For<Func<DataProviderType, ILeagueService>>();
            leagueServiceFactory(DataProviderType.SportRadar).Returns(leagueService);

            fetchLeaguesSeason = new FetchLeaguesSeasonTask(messageBus, appSettings, leagueServiceFactory);
        }

        [Fact]
        public async Task FetchLeaguesSeason_Empty_NotPublishAnyMessages()
        {
            appSettings.ScheduleTasksSettings.Returns(A.Dummy<ScheduleTasksSettings>());

            await fetchLeaguesSeason.FetchLeaguesSeason();

            await messageBus.DidNotReceive().Publish(Arg.Any<LeaguesSeasonFetchedMessage>());
        }

        [Fact]
        public async Task FetchLeaguesSeason_PublishMessages()
        {
            appSettings.ScheduleTasksSettings.Returns(A.Dummy<ScheduleTasksSettings>().With(setting => setting.QueueBatchSize, 3));
            leagueService.GetLeagues(Arg.Any<Language>()).Returns(A.CollectionOfDummy<League>(5));

            await fetchLeaguesSeason.FetchLeaguesSeason();

            await messageBus.Received(2).Publish(Arg.Any<LeaguesSeasonFetchedMessage>());
        }
    }
}
