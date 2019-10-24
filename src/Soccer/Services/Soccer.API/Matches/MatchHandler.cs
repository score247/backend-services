namespace Soccer.API.Matches
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Teams.Models;

    public class MatchHandler :
        IRequestHandler<MatchesByDateRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<MatchInfoByIdRequest, MatchInfo>,
        IRequestHandler<LiveMatchesRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<LiveMatchCountRequest, int>,
        IRequestHandler<MatchCoverageByIdRequest, MatchCoverage>,
        IRequestHandler<MatchCommentaryByIdRequest, IEnumerable<MatchCommentary>>,
        IRequestHandler<MatchStatisticRequest, MatchStatistic>,
        IRequestHandler<MatchLineupsRequest, MatchLineups>
    {
        private readonly IMatchQueryService matchQueryService;

        public MatchHandler(IMatchQueryService matchQueryService)
        {
            this.matchQueryService = matchQueryService;
        }

        public async Task<IEnumerable<MatchSummary>> Handle(MatchesByDateRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetByDateRange(request.From, request.To, request.Language);

        public async Task<IEnumerable<MatchSummary>> Handle(LiveMatchesRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetLive(request.Language);

        public async Task<MatchInfo> Handle(MatchInfoByIdRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetMatchInfo(request.Id, request.Language);

        public async Task<int> Handle(LiveMatchCountRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetLiveMatchCount(request.Language);

        public async Task<MatchCoverage> Handle(MatchCoverageByIdRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetMatchCoverage(request.Id, request.Language);

        public Task<IEnumerable<MatchCommentary>> Handle(MatchCommentaryByIdRequest request, CancellationToken cancellationToken)
            => matchQueryService.GetMatchCommentary(request.Id, request.Language);

        public async Task<MatchStatistic> Handle(MatchStatisticRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetMatchStatistic(request.Id);

        public async Task<MatchLineups> Handle(MatchLineupsRequest request, CancellationToken cancellationToken)
        {
            var match = await matchQueryService.GetMatchLineups(request.Id, request.Language);
            // Please Ignore this temporaty code here
            return match; //new MatchLineups(StubLineUp(true), StubLineUp(false), string.Empty);
        }
    }
}