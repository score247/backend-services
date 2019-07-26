namespace Soccer.API.Matches
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Soccer.API.Matches.Requests;
    using Soccer.Core.Matches.Models;

    public class MatchHandler
        : IRequestHandler<MatchesByDateRequest, IEnumerable<Match>>,
          IRequestHandler<LiveMatchesRequest, IEnumerable<Match>>
    {
        private readonly IMatchQueryService matchQueryService;

        public MatchHandler(IMatchQueryService matchQueryService)
        {
            this.matchQueryService = matchQueryService;
        }

        public async Task<IEnumerable<Match>> Handle(MatchesByDateRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetByDateRange(request.From, request.To, request.ClientTimeOffset, request.Language);

        public async Task<IEnumerable<Match>> Handle(LiveMatchesRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetLive(request.ClientTimeOffset, request.Language);
    }
}