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
        [Route("major/cleancache")]
        public async Task<bool> CleanMajorLeaguesCache()
            => await mediator.Send(new CleanMajorLeaguesRequest());

        [HttpGet, AllowAnonymous]
        [Route("season/unprocessed")]
        public async Task<IEnumerable<LeagueSeasonProcessedInfo>> GetUnprocessedSeasons(string language = Language.English)
            => await mediator.Send(new UnprocessedLeagueSeasonRequest());

        [HttpGet]
        [Route("{id}/seasons/{seasonId}/matches/{leagueGroupName}")]
        public async Task<IEnumerable<MatchSummary>> GetMatches(
              string id,
              string seasonId,
              string leagueGroupName,
              string language = Language.English)
          => await mediator.Send(new MatchesByLeagueRequest(id, seasonId, leagueGroupName, language));

        [HttpGet]
        [Route("{id}/seasons/{seasonId}/table/{groupName}")]
        public async Task<LeagueTable> GetLeagueTable(string id, string seasonId, string groupName, string language = Language.English)
            => await mediator.Send(new LeagueTableRequest(id, seasonId, groupName, language));

        [HttpGet, AllowAnonymous]
        [HttpGet]
        [Route("country/{countryCode}")]
        public async Task<IEnumerable<League>> GetCountryLeagues(string countryCode, string language = Language.English)
            => await mediator.Send(new CountryLeaguesRequest(countryCode, language));

        [HttpGet, AllowAnonymous]
        [HttpGet]
        [Route("{leagueId}/seasons/{seasonId}/groups")]
        public async Task<IEnumerable<LeagueGroupStage>> GetLeagueGroups(string leagueId, string seasonId, string language = Language.English)
            => await mediator.Send(new LeagueGroupsRequest(leagueId, seasonId, language));

        [HttpGet, AllowAnonymous]
        [Route("updateabbreviation")]
        public async Task<bool> UpdateLeagueAbbreviation()
            => await mediator.Send(new UpdateLeagueAbbreviationRequest());
    }
}