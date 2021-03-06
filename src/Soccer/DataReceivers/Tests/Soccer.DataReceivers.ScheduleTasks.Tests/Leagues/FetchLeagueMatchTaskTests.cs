using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;
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
            var leagueFactory = Substitute.For<Func<DataProviderType, ILeagueService>>();

            appSettings.ScheduleTasksSettings.Returns(A.Dummy<ScheduleTasksSettings>());

            fetchLeagueMatchesTask = new FetchLeagueMatchesTask(messageBus, appSettings, leagueScheduleService, leagueSeasonService, jobClient, leagueFactory);
        }

        [Fact]
        public async Task FetchLeagueMatches_Always_GetUnprocessedLeagueSeason()
        {
            // Arrange

            // Act
            await fetchLeagueMatchesTask.FetchLeagueMatchesAndTimelineEvents();

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
            await fetchLeagueMatchesTask.FetchLeagueMatchesAndTimelineEvents();

            // Assert
            jobClient.DidNotReceive().Enqueue<IFetchLeagueMatchesTask>(t => t.FetchMatchesForNewSeason(unprocessedLeagues, true));
        }

        [Fact]
        public async Task FetchLeagueMatches_HasUnprocessedLeagueSeason_EnqueueTasks()
        {
            // Arrange
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(3);
            leagueSeasonService.GetUnprocessedLeagueSeason().Returns(unprocessedLeagues);

            // Act
            await fetchLeagueMatchesTask.FetchLeagueMatchesAndTimelineEvents();

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
            await fetchLeagueMatchesTask.FetchLeagueMatchesAndTimelineEvents();

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
            await fetchLeagueMatchesTask.FetchMatchesForNewSeason(unprocessedLeagues);

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
            await fetchLeagueMatchesTask.FetchMatchesForNewSeason(unprocessedLeagues);

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
            await fetchLeagueMatchesTask.FetchMatchesForNewSeason(unprocessedLeagues);

            // Assert
            await messageBus.Received(2).Publish<IPreMatchesFetchedMessage>(Arg.Any<PreMatchesFetchedMessage>());
        }

        [Fact]
        public async Task FetchMatchesForLeague_HasClosedMatch_ScheduleToFetchTimelineAndLineups()
        {
            // Arrange
            appSettings.ScheduleTasksSettings.Returns(new ScheduleTasksSettings { QueueBatchSize = 5 });
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(1);

            leagueScheduleService
                .GetLeagueMatches(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>())
                .Returns(new List<Match>
                {
                    StubMatchWithEventStatus(MatchStatus.Closed),
                    StubMatchWithEventStatus(MatchStatus.NotStarted),
                    StubMatchWithEventStatus(MatchStatus.Cancelled),
                });

            // Act
            await fetchLeagueMatchesTask.FetchMatchesForNewSeason(unprocessedLeagues);

            // Assert
            jobClient
                .Received(1)
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchTimelineTask.FetchTimelineEventsForClosedMatch)), Arg.Any<ScheduledState>());

            jobClient
               .Received(1)
               .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchMatchLineupsTask.FetchMatchLineupsForClosedMatch)), Arg.Any<ScheduledState>());
        }

        [Fact]
        public async Task FetchMatchesForLeague_Always_ScheduleTeamResultsTasksForDistinctTeams()
        {
            // Arrange
            appSettings.ScheduleTasksSettings.Returns(new ScheduleTasksSettings { QueueBatchSize = 5 });
            var unprocessedLeagues = A.CollectionOfDummy<LeagueSeasonProcessedInfo>(1);

            leagueScheduleService
                .GetLeagueMatches(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>())
                .Returns(new List<Match>
                {
                    StubMatchWithTeam("team:1", "team:2"),
                    StubMatchWithTeam("team:3", "team:4"),
                    StubMatchWithTeam("team:1", "team:4"),
                    StubMatchWithTeam("team:2", "team:3"),
                    StubMatchWithTeam("team:1", "team:3")
                });

            // Act
            await fetchLeagueMatchesTask.FetchMatchesForNewSeason(unprocessedLeagues);

            // Assert
            jobClient
                .Received(1)
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchHeadToHeadsTask.FetchTeamResults)), Arg.Any<ScheduledState>());
        }

        private static Match StubMatchWithEventStatus(MatchStatus status)
            => A.Dummy<Match>().With(match => match.MatchResult, A.Dummy<MatchResult>().With(result => result.EventStatus, status));

        private static Match StubMatchWithTeam(string homeTeamId, string awayTeamId)
           => A.Dummy<Match>().With(match => match.Teams, new List<Team>
           {
                A.Dummy<Team>().With(team => team.Id, homeTeamId).With(team=>team.IsHome, true),
                A.Dummy<Team>().With(team => team.Id, awayTeamId).With(team=>team.IsHome, false)
           });
    }
}