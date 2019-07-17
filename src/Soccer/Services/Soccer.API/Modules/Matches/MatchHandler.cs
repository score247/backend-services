namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Soccer.API.Modules.Matches.Queries;
    using Soccer.Core.Domain.Matches.Models;

    public class MatchHandler
        : IRequestHandler<GetMatchesByDateQuery, IEnumerable<Match>>,
          IRequestHandler<GetLiveMatchesQuery, IEnumerable<Match>>
    {
        private readonly IMatchQueryService matchQueryService;

        public MatchHandler(IMatchQueryService matchQueryService)
        {
            this.matchQueryService = matchQueryService;
        }

        public async Task<IEnumerable<Match>> Handle(GetMatchesByDateQuery request, CancellationToken cancellationToken)
            => await matchQueryService.GetByDateRange(request.From, request.To, request.ClientTimeZone, request.Language);

        public Task<IEnumerable<Match>> Handle(GetLiveMatchesQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}