using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FakeItEasy;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
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

        [Fact]
        public async Task FetchLeagueMatches_NotHaveUnprocessedLeagueSeason_NotEnqueueTasks()
        {
            // Arrange
            var unprocessedLeagues = new List<LeagueSeasonProcessedInfo>();
            leagueSeasonService.GetUnprocessedLeagueSeason().Returns(unprocessedLeagues);

            // Act
            await fetchLeagueMatchesTask.FetchLeagueMatches();

            // Assert
            jobClient.DidNotReceive().Enqueue<IFetchLeagueMatchesTask>(t => t.FetchMatchesForLeague(unprocessedLeagues));
        }

        [Fact]
        public async Task FetchLeagueMatches_HasUnprocessedLeagueSeason_EnqueueTasks()
        {
            // Arrange
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(3);
            leagueSeasonService.GetUnprocessedLeagueSeason().Returns(unprocessedLeagues);

            // Act
            await fetchLeagueMatchesTask.FetchLeagueMatches();

            // Assert
            jobClient.Received(1).Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
        }

        [Fact]
        public async Task FetchLeagueMatches_HasUnprocessedLeagueSeason_EnqueueManyTasks()
        {
            // Arrange
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(7);
            leagueSeasonService.GetUnprocessedLeagueSeason().Returns(unprocessedLeagues);

            // Act
            await fetchLeagueMatchesTask.FetchLeagueMatches();

            // Assert
            jobClient.Received(2).Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
        }

        [Fact]
        public async Task FetchMatchesForLeague_Always_GetGetLeagueMatches()
        {
            // Arrange
            appSettings.ScheduleTasksSettings.Returns(new ScheduleTasksSettings());
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(5);

            // Act
            await fetchLeagueMatchesTask.FetchMatchesForLeague(unprocessedLeagues);

            // Assert
            await leagueScheduleService.Received(5).GetLeagueMatches(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>());
        }

        [Fact]
        public async Task FetchMatchesForLeague_Always_PublishPreMatchesMessage()
        {
            // Arrange
            appSettings.ScheduleTasksSettings.Returns(new ScheduleTasksSettings { QueueBatchSize = 5 });
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(1);

            leagueScheduleService
                .GetLeagueMatches(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>())
                .Returns(A.CollectionOfDummy<Match>(5));

            // Act
            await fetchLeagueMatchesTask.FetchMatchesForLeague(unprocessedLeagues);

            // Assert
            await messageBus.Received(1).Publish<IPreMatchesFetchedMessage>(Arg.Any<PreMatchesFetchedMessage>());
        }

        [Fact]
        public async Task FetchMatchesForLeague_Always_PublishManyPreMatchesMessage()
        {
            // Arrange
            appSettings.ScheduleTasksSettings.Returns(new ScheduleTasksSettings { QueueBatchSize = 5 });
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(1);

            leagueScheduleService
                .GetLeagueMatches(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>())
                .Returns(A.CollectionOfDummy<Match>(7));

            // Act
            await fetchLeagueMatchesTask.FetchMatchesForLeague(unprocessedLeagues);

            // Assert
            await messageBus.Received(2).Publish<IPreMatchesFetchedMessage>(Arg.Any<PreMatchesFetchedMessage>());
        }
    }
}
