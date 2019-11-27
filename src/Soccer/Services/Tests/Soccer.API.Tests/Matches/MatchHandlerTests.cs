using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NSubstitute;
using Soccer.API.Matches;
using Soccer.API.Matches.Helpers;
using Soccer.API.Matches.Requests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Xunit;

namespace Soccer.API.Tests.Matches
{
    public class MatchHandlerTests
    {
        private readonly MatchHandler matchHandler;
        private readonly IMatchQueryService matchQueryService;
        private readonly IMatchLineupsGenerator matchLineupsGenerator;

        public MatchHandlerTests()
        {
            matchQueryService = Substitute.For<IMatchQueryService>();
            matchLineupsGenerator = Substitute.For<IMatchLineupsGenerator>();

            matchHandler = new MatchHandler(matchQueryService, matchLineupsGenerator);
        }

        [Fact]
        public async Task HandleMatchLineupsRequest_MatchLineupsHasData_ReturnMatchPitchViewLineups()
        {
            var matchId = "match-id";
            var request = new MatchLineupsRequest(matchId, Language.en_US.DisplayName);
            var matchLineups = new MatchLineups(matchId, DateTimeOffset.Now, A.Dummy<TeamLineups>(), A.Dummy<TeamLineups>());
            matchQueryService
                .GetMatchLineups(request.Id, request.Language)
                .Returns(Task.FromResult(matchLineups));
            matchLineupsGenerator.Generate(Arg.Is<MatchLineups>(matchLineups => matchLineups.Id == matchId)).Returns("matchlineup-svg");

            var actualMatchLineups = await matchHandler.Handle(request, new CancellationToken());

            Assert.Equal(matchId, actualMatchLineups.Id);
            Assert.Equal(matchLineups.EventDate, actualMatchLineups.EventDate);
            Assert.Equal(matchLineups.Home, actualMatchLineups.Home);
            Assert.Equal(matchLineups.Away, actualMatchLineups.Away);
            Assert.Equal("matchlineup-svg", actualMatchLineups.PitchView);
        }
#pragma warning disable S2699 // Tests should include assertions
        [Fact]
        public async Task HandleMatchesByDateRequest_Execute_GetByDateRange()
        {
            var fromDate = new DateTime(2019, 1, 2);
            var toDate = new DateTime(2019, 1, 3);
            var request = new MatchesByDateRequest(fromDate, toDate, Language.English);

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetByDateRange(fromDate, toDate, request.Language);
        }

        [Fact]
        public async Task HandleLiveMatchesRequest_Execute_GetLive()
        {
            var request = new LiveMatchesRequest(Language.English);

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetLive(request.Language);
        }


        [Fact]
        public async Task HandleMatchInfoByIdRequest_Execute_GetMatchInfo()
        {
            var matchId = "MatchID";
            var request = new MatchInfoByIdRequest(matchId, Language.English);

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetMatchInfo(matchId, request.Language, request.EventDate);
        }

        [Fact]
        public async Task HandleLiveMatchCountRequest_Execute_GetLiveMatchCount()
        {
            var request = new LiveMatchCountRequest();

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetLiveMatchCount(request.Language);
        }

        [Fact]
        public async Task HandleMatchCoverageByIdRequest_Execute_GetMatchCoverage()
        {
            var matchId = "MatchID";
            var request = new MatchCoverageByIdRequest(matchId, Language.English);

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetMatchCoverage(matchId, request.Language);
        }

        [Fact]
        public async Task HandleMatchCommentaryByIdRequest_Execute_GetMatchCommentary()
        {
            var matchId = "MatchID";
            var request = new MatchCommentaryByIdRequest(matchId, Language.English);

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetMatchCommentary(matchId, request.Language);
        }


        [Fact]

        public async Task HandleMatchStatisticRequest_Execute_GetMatchStatistic()

        {
            var matchId = "MatchID";
            var request = new MatchStatisticRequest(matchId);

            await matchHandler.Handle(request, new CancellationToken());

            await matchQueryService.Received(1).GetMatchStatistic(matchId);
        }
#pragma warning restore S2699 // Tests should include assertions
    }
}