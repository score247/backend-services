namespace Soccer.API.Odds
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;

    [Route("api/soccer/")]
    [ApiController]
    public class OddsController : ControllerBase
    {
        private readonly IOddsQueryService oddsQueryService;

        public OddsController(
            IOddsQueryService oddsQueryService)
        {
            this.oddsQueryService = oddsQueryService;
        }

        [HttpGet("{lang}/odds/{matchId}/{betTypeId}/{format}")]
        public async Task<MatchOdds> Get(string matchId, int betTypeId, string lang = "en-US", string format = "dec")
            => await oddsQueryService.GetOdds(matchId, betTypeId, Enumeration.FromDisplayName<Language>(lang));

        [HttpGet("{lang}/odds-movement/{matchId}/{betTypeId}/{format}/{bookmakerId}")]
        public async Task<MatchOddsMovement> OddsMovement(string matchId, int betTypeId, string bookmakerId, string lang = "en-US", string format = "dec")
            => await oddsQueryService.GetOddsMovement(matchId, betTypeId, bookmakerId, Enumeration.FromDisplayName<Language>(lang));
    }
}