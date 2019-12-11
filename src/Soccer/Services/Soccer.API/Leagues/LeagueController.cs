using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet, AllowAnonymous]
        [Route("major")]
        public async Task<IEnumerable<League>> GetMajorLeagues(string language = Language.English)
            => await mediator.Send(new MajorLeaguesRequest(language));

        [HttpGet, AllowAnonymous]
        [Route("season/unprocessed")]
        public async Task<IEnumerable<LeagueSeasonProcessedInfo>> GetUnprocessedSeasons(string language = Language.English)
            => await mediator.Send(new UnprocessedLeagueSeasonRequest());

        [HttpGet]
        [Route("{id}/matches/{leagueGroupName}")]
        public async Task<IEnumerable<MatchSummary>> GetMatches(
              string id,
              string leagueGroupName,
              string language = Language.English)
          => await mediator.Send(new MatchesByLeagueRequest(id, leagueGroupName, language));

        [HttpGet]
        [Route("{id}/season/{seasonId}/table/{groupName}")]
        public async Task<LeagueTable> GetLeagueTable(string id, string seasonId, string groupName, string language = Language.English)
            => await mediator.Send(new LeagueTableRequest(id, seasonId, groupName, language));
    }
}