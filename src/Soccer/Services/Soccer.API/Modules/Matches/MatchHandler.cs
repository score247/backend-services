namespace Soccer.API.Modules.Matches
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Soccer.API.Modules.Matches.Requests;
    using Soccer.Core.Domain.Matches.Models;

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