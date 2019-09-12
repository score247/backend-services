namespace Soccer.API.Odds
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Soccer.API.Odds.Requests;
    using Soccer.Core.Odds.Models;

    [Route("api/soccer/")]
    [ApiController]
    public class OddsController : ControllerBase
    {
        private readonly IMediator mediator;

        public OddsController(IMediator mediator)
            => this.mediator = mediator;

        [HttpGet("{lang}/odds/{matchId}/{betTypeId}/{format}")]
        public async Task<MatchOdds> Get(string matchId, int betTypeId, string lang = "en-US", string format = "dec")
            => await mediator.Send(new OddsRequest(matchId, betTypeId, lang, format));

        [HttpGet("{lang}/odds-movement/{matchId}/{betTypeId}/{format}/{bookmakerId}")]
        public async Task<MatchOddsMovement> OddsMovement(string matchId, int betTypeId, string bookmakerId, string lang = "en-US", string format = "dec")
            => await mediator.Send(new OddsMovementRequest(matchId, betTypeId, bookmakerId, lang, format));
    }
}