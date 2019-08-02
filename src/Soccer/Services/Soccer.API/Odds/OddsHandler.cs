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
        private readonly IOddsQueryService oddsQueryService;

        public OddsHandler(
            IOddsQueryService oddsQueryService)
        {
            this.oddsQueryService = oddsQueryService;
        }

        public Task<MatchOdds> Handle(OddsRequest request, CancellationToken cancellationToken)
            => oddsQueryService.GetOdds(request.MatchId, request.BetTypeId, request.Languge);

        public Task<MatchOddsMovement> Handle(OddsMovementRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}