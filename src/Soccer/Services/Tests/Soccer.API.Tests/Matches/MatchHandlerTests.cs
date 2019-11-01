////using System;
////using System.Collections.Generic;
////using System.Text;
////using NSubstitute;
////using Soccer.API.Matches;
////using Soccer.API.Matches.Helpers;
////using Soccer.API.Matches.Requests;

////namespace Soccer.API.Tests.Matches
////{
////    public class MatchHandlerTests
////    {
////        private readonly MatchHandler matchHandler;
////        private readonly IMatchQueryService matchQueryService;
////        private readonly IMatchLineupsGenerator matchLineupsGenerator;

////        public MatchHandlerTests()
////        {
////            matchQueryService = Substitute.For<IMatchQueryService>();
////            matchLineupsGenerator = Substitute.For<IMatchLineupsGenerator>();

////            matchHandler = new MatchHandler(matchQueryService, matchLineupsGenerator);
////        }

////        public void Handle_MatchLineupsHasData_ReturnMatchPitchViewLineups()
////        {
////            var request = new MatchLineupsRequest("match-id", "en-us");
////            var matchLineups = new MatchLineups

////            matchQueryService
////                .GetMatchLineups(request.Id, request.Language)
////                .Returns();

////        }
////    }
////}
