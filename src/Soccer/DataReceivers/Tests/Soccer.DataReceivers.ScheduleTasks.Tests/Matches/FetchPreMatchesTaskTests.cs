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
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Matches
{
    public class FetchPreMatchesTaskTests
    {
        private readonly IAppSettings appSettings;
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly ILeagueService internalLeagueService;
        private readonly IBackgroundJobClient jobClient;
        private readonly List<League> majorLeagues;

        private readonly FetchPreMatchesTask fetchPreMatchesTask;

        public FetchPreMatchesTaskTests()
        {
            appSettings = Substitute.For<IAppSettings>();
            matchService = Substitute.For<IMatchService>();
            messageBus = Substitute.For<IBus>();
            internalLeagueService = Substitute.For<ILeagueService>();
            jobClient = Substitute.For<IBackgroundJobClient>();

            var leagueServiceFactory = Substitute.For<Func<DataProviderType, ILeagueService>>();
            leagueServiceFactory(DataProviderType.Internal).Returns(internalLeagueService);
            appSettings.ScheduleTasksSettings.Returns(A.Dummy<ScheduleTasksSettings>().With(setting => setting.FetchMatchesByDateDelayedHours, 1));
            majorLeagues = new List<League>
            {
                StubLeague("major:1"),
                StubLeague("major:1")
            };

            fetchPreMatchesTask = new FetchPreMatchesTask(messageBus, appSettings, matchService, leagueServiceFactory, jobClient);
        }

        [Fact]
        public async Task FetchPreMatches_EmptyMajorLeague_NotScheduleAnyFetchJob() 
        {
            await fetchPreMatchesTask.FetchPreMatches(3);

            jobClient
                .DidNotReceive()
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchPreMatchesTask.FetchPreMatchesForDate)), Arg.Any<ScheduledState>());
        }

        [Fact]
        public async Task FetchPreMatches_HasMajorLeague_ScheduleFetchJobByDate()
        {
            internalLeagueService.GetLeagues(Language.en_US).Returns(majorLeagues);

            await fetchPreMatchesTask.FetchPreMatches(3);

            jobClient
                .Received(3)
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchPreMatchesTask.FetchPreMatchesForDate)), Arg.Any<ScheduledState>());
        }

        [Fact]
        public async Task FetchPreMatchesForDate_NotHaveMatch_NotPublishMessageAndEnqueueTask()
        {           
            await fetchPreMatchesTask.FetchPreMatchesForDate(DateTime.Now, Arg.Any<Language>(), majorLeagues);

            await messageBus
                .DidNotReceive()
                .Publish<IPreMatchesFetchedMessage>(Arg.Any<PreMatchesFetchedMessage>());

            jobClient
                .DidNotReceive()
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchPreMatchesTimelineTask.FetchPreMatchTimeline)), Arg.Any<EnqueuedState>());
        }

        [Fact]
        public async Task FetchPreMatchesForDate_Always_FetchPreMatch()
        {
            var fetchDate = DateTime.Now;

            await fetchPreMatchesTask.FetchPreMatchesForDate(fetchDate, Language.en_US, majorLeagues);

            await matchService.Received(1).GetPreMatches(Arg.Is<DateTime>(date => date == fetchDate), Language.en_US);
        }

        [Fact]
        public async Task FetchPreMatchesForDate_MatchNotInMajor_NotPublishMessageAndEnqueueTask()
        {
            var fetchDate = DateTime.Now;
            matchService.GetPreMatches(Arg.Is<DateTime>(date => date == fetchDate), Language.en_US).Returns(new List<Match> 
            {
                A.Dummy<Match>().With(match => match.League, StubLeague("league:1"))
            });

            await fetchPreMatchesTask.FetchPreMatchesForDate(fetchDate, Language.en_US, majorLeagues);

            await messageBus
               .DidNotReceive()
               .Publish<IPreMatchesFetchedMessage>(Arg.Any<PreMatchesFetchedMessage>());

            jobClient
                .DidNotReceive()
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchPreMatchesTimelineTask.FetchPreMatchTimeline)), Arg.Any<EnqueuedState>());
        }

        [Fact]
        public async Task FetchPreMatchesForDate_MatchInMajor_PublishMessageAndEnqueueTask()
        {
            var fetchDate = DateTime.Now;
            var match = A.Dummy<Match>().With(match => match.League, StubLeague("major:1"));
            matchService
                .GetPreMatches(Arg.Any<DateTime>(), Language.en_US)
                .Returns(new List<Match>
                {
                    match
                }); 

            await fetchPreMatchesTask.FetchPreMatchesForDate(fetchDate, Language.en_US, majorLeagues);

            await messageBus
               .Received(1)
               .Publish<IPreMatchesFetchedMessage>(Arg.Any<PreMatchesFetchedMessage>());

            jobClient
                .Received(1)
                .Create(Arg.Is<Job>(job => job.Method.Name == nameof(IFetchPreMatchesTimelineTask.FetchPreMatchTimeline)), Arg.Any<EnqueuedState>());
        }

        private League StubLeague(string id)
            => A.Dummy<League>().With(league => league.Id, id);
    }
}
