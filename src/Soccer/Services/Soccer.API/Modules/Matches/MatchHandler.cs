namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Soccer.API.Modules.Matches.Queries;
    using Soccer.Core.Domain.Matches;

    public class MatchHandler
        : IRequestHandler<GetMatchesByDateQuery, IEnumerable<Match>>,
          IRequestHandler<GetLiveMatchesQuery, IEnumerable<Match>>
    {
        public Task<IEnumerable<Match>> Handle(GetMatchesByDateQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Match>> Handle(GetLiveMatchesQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}