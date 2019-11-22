using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Leagues
{
    [Trait("Soccer.DataReceivers.ScheduleTasks", "FetchLeagueMatchTask")]
    public class FetchLeagueMatchTaskTests
    {
        private readonly ILeagueScheduleService leagueScheduleService;
        private readonly ILeagueSeasonService leagueSeasonService;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;
        private readonly IBackgroundJobClient jobClient;

        private readonly FetchLeagueMatchesTask fetchLeagueMatchesTask;

        public FetchLeagueMatchTaskTests() 
        {
            leagueScheduleService = Substitute.For<ILeagueScheduleService>();
            leagueSeasonService = Substitute.For<ILeagueSeasonService>();
            appSettings = Substitute.For<IAppSettings>();
            messageBus = Substitute.For<IBus>();
            jobClient = Substitute.For<IBackgroundJobClient>();

            fetchLeagueMatchesTask = new FetchLeagueMatchesTask(messageBus, appSettings, leagueScheduleService, leagueSeasonService, jobClient);
        }

        [Fact]
        public async Task FetchLeagueMatches_Always_GetUnprocessedLeagueSeason() 
        {
            // Arrange

            // Act
            await fetchLeagueMatchesTask.FetchLeagueMatches();

            // Assert
            leagueSeasonService.Received(1).GetUnprocessedLeagueSeason();
        }

    }
}
