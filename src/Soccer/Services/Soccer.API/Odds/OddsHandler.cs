namespace Soccer.API.Odds
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
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
            => oddsQueryService.GetOddsMovement(request.MatchId, request.BetTypeId, request.BookmakerId, request.Languge);
    }
}