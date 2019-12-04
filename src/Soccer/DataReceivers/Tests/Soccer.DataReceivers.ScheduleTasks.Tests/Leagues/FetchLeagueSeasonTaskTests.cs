using System;
using MassTransit;
using NSubstitute;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
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

        public FetchLeagueSeasonTaskTests() 
        {
            appSettings = Substitute.For<IAppSettings>();
            messageBus = Substitute.For<IBus>();
            leagueServiceFactory = Substitute.For<Func<DataProviderType, ILeagueService>>();
        }
    }
}
