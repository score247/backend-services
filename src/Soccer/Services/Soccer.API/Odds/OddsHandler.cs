namespace Soccer.API.Odds
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MediatR;
    using Soccer.API.Matches;
    using Soccer.API.Odds.Requests;
    using Soccer.Core.Odds.Models;

    public class OddsHandler : 
        IRequestHandler<OddsRequest, MatchOdds>,
        IRequestHandler<OddsMovementRequest, MatchOddsMovement>
    {
        

        public Task<MatchOdds> Handle(OddsRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MatchOddsMovement> Handle(OddsMovementRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}