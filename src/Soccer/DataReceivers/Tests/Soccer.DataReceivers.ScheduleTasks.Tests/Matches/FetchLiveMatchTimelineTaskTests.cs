using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Matches
{
    public class FetchLiveMatchTimelineTaskTests
    {
        private readonly IMatchService matchService;
        private readonly IFetchMatchLineupsTask fetchMatchLineupsTask;
        private readonly IFetchTimelineTask fetchTimelineTask;
        private readonly ILeagueService internalLeagueService;
        private readonly Func<DataProviderType, ILeagueService> leagueServiceFactory;

        private readonly FetchLiveMatchesTimelineTask fetchLiveMatchesTimelineTask;

        public FetchLiveMatchTimelineTaskTests()
        {
            matchService = Substitute.For<IMatchService>();
            fetchMatchLineupsTask = Substitute.For<IFetchMatchLineupsTask>();
            fetchTimelineTask = Substitute.For<IFetchTimelineTask>();
            internalLeagueService = Substitute.For<ILeagueService>();

            leagueServiceFactory = Substitute.For<Func<DataProviderType, ILeagueService>>();
            leagueServiceFactory(DataProviderType.Internal).Returns(internalLeagueService);

            fetchLiveMatchesTimelineTask = new FetchLiveMatchesTimelineTask(matchService, fetchMatchLineupsTask, fetchTimelineTask, leagueServiceFactory);
        }

        [Fact]
        public async Task FetchLiveMatchesTimeline_EmptyMajorLeagues_NotFetchLiveMatchApi()
        {
            await fetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline();

            await matchService.DidNotReceive().GetLiveMatches(Arg.Any<Language>());
        }

        [Fact]
        public async Task FetchLiveMatchesTimeline_HasMajorLeagues_FetchLiveMatchApi()
        {
            internalLeagueService.GetLeagues(Arg.Any<Language>()).Returns(A.CollectionOfDummy<League>(5));

            await fetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline();

            await matchService.Received(1).GetLiveMatches(Arg.Any<Language>());
        }

        [Fact]
        public async Task FetchLiveMatchesTimeline_NoLiveMatchInMajor_NotFetchTimeline()
        {
            internalLeagueService.GetLeagues(Arg.Any<Language>()).Returns(new List<League>
            {
                A.Dummy<League>().With(league => league.Id, "major:league")
            });         

            matchService.GetLiveMatches(Arg.Any<Language>()).Returns(new List<Match>
            {
                 StubMatch("not-major:league", MatchStatus.Live)
            });

            await fetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline();

            await fetchTimelineTask.DidNotReceive().FetchTimelines(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>());
        }

        [Fact]
        public async Task FetchLiveMatchesTimeline_LiveMatchInMajor_FetchTimeline()
        {
            internalLeagueService.GetLeagues(Arg.Any<Language>()).Returns(new List<League>
            {
                A.Dummy<League>().With(league => league.Id, "major:league")
            });

            matchService.GetLiveMatches(Arg.Any<Language>()).Returns(new List<Match>
            {
                 StubMatch("major:league", MatchStatus.Live)
            });

            await fetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline();

            await fetchTimelineTask.Received(1).FetchTimelines(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>());
        }

        [Fact]
        public async Task FetchLiveMatchesTimeline_LiveMatchInMajorAndClosed_NotFetchLineUps()
        {
            internalLeagueService.GetLeagues(Arg.Any<Language>()).Returns(new List<League>
            {
                A.Dummy<League>().With(league => league.Id, "major:league")
            });

            matchService.GetLiveMatches(Arg.Any<Language>()).Returns(new List<Match>
            {
                 StubMatch("major:league", MatchStatus.Closed)
            });

            await fetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline();

            await fetchMatchLineupsTask.DidNotReceive().FetchMatchLineups(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>());
        }

        [Fact]
        public async Task FetchLiveMatchesTimeline_LiveMatchInMajor_FetchLineUpsForNotStartedAndLive()
        {
            internalLeagueService.GetLeagues(Arg.Any<Language>()).Returns(new List<League>
            {
                A.Dummy<League>().With(league => league.Id, "major:league")
            });

            matchService.GetLiveMatches(Arg.Any<Language>()).Returns(new List<Match>
            {
                 StubMatch("major:league", MatchStatus.Closed),
                 StubMatch("major:league", MatchStatus.NotStarted),
                 StubMatch("major:league", MatchStatus.Live)
            });

            await fetchLiveMatchesTimelineTask.FetchLiveMatchesTimeline();

            await fetchMatchLineupsTask.Received(2).FetchMatchLineups(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>());
        }

        private static Match StubMatch(string leagueId, MatchStatus eventStatus)
        => A.Dummy<Match>()
            .With(match => match.League, A.Dummy<League>().With(league => league.Id, leagueId))
            .With(match => match.MatchResult, A.Dummy<MatchResult>().With(result => result.EventStatus, eventStatus));

    }
}
