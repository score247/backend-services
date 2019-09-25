namespace Soccer.API.Matches
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Soccer.API.Matches.Models;
    using Soccer.API.Matches.Requests;
    using Soccer.Core.Matches.Models;

    public class MatchHandler :   
        IRequestHandler<MatchesByDateRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<MatchInfoByIdRequest, MatchInfo>,
        IRequestHandler<LiveMatchesRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<LiveMatchCountRequest, int>
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
    }
}