using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.Leagues.Requests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues
{
    [Route("api/soccer/{language}/leagues")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly IMediator mediator;

        public LeagueController(IMediator mediator)
            => this.mediator = mediator;

        [HttpGet]
        [Route("major")]
        public async Task<IEnumerable<League>> GetMajorLeagues(string language = Language.English)
            => await mediator.Send(new MajorLeaguesRequest(language));

        [HttpGet]
        [Route("season/unprocessed")]
        public async Task<IEnumerable<LeagueSeasonProcessedInfo>> GetUnprocessedSeasons(string language = Language.English)
            => await mediator.Send(new UnprocessedLeagueSeasonRequest());

        [HttpGet]
        [Route("{id}/matches")]
        public async Task<IEnumerable<MatchSummary>> GetMatches(
              string id,
              string language = Language.English)
          => await mediator.Send(new MatchesByLeagueRequest(id, language));
    }
}