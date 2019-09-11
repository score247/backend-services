namespace Soccer.API.Odds
{
    using System;
    using System.Collections.Generic;
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
        //=> await mediator.Send(new OddsRequest(matchId, betTypeId, lang, format));
        {
            var betOption = new BetOptionOdds("type", 1.1m, 2.2m, "3.3", "4.4");
            betOption.ResetLiveOddsToOpeningOdds();
            var betOptions = new List<BetOptionOdds>
                        {
                            betOption
                        };
            var matchOdds = new MatchOdds(
                "match id",
                new List<BetTypeOdds>
                {
                    new BetTypeOdds(
                        1,
                        "name",
                        new Bookmaker("bookmaker", "bookmaker name"),
                        DateTime.Now,
                        betOptions
                        )
                },
                DateTime.Now);

            return await Task.FromResult(matchOdds);
        }

        [HttpGet("{lang}/odds-movement/{matchId}/{betTypeId}/{format}/{bookmakerId}")]
        public async Task<MatchOddsMovement> OddsMovement(string matchId, int betTypeId, string bookmakerId, string lang = "en-US", string format = "dec")
        //    => await mediator.Send(new OddsMovementRequest(matchId, betTypeId, bookmakerId, lang, format));
        {
            var betOption = new BetOptionOdds("type", 1.1m, 2.2m, "3.3", "4.4");
            betOption.ResetLiveOddsToOpeningOdds();
            var betOptions = new List<BetOptionOdds>
                        {
                            betOption
                        };

            return await Task.FromResult(
                new MatchOddsMovement(
                    "match id",
                    new Bookmaker("bookmaker", "bookmaker name"),
                    new List<OddsMovement>
                    {
                        new OddsMovement(
                            betOptions,
                            "Live",
                            DateTimeOffset.Now,
                            true, 
                            1, 
                            3)
                    }));
        }
    }
}