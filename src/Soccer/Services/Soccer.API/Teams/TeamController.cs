using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.Teams.Requests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Teams
{
    [Route("api/soccer/{language}/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMediator mediator;

        public TeamController(IMediator mediator)
            => this.mediator = mediator;

        /// <summary>
        /// Get Head To Head Matches
        /// </summary>
        [HttpGet]
        [Route("{homeTeamId}/versus/{awayTeamId}")]
        public async Task<IEnumerable<MatchSummary>> GetHeadToHeads(
            string homeTeamId,
            string awayTeamId,
            string language = "en-US")
            => await mediator.Send(new HeadToHeadRequest(homeTeamId, awayTeamId, language));

        /// <summary>
        /// Get Team Result Matches
        /// </summary>
        [HttpGet]
        [Route("{teamId}/results")]
        public async Task<IEnumerable<MatchSummary>> GetTeamResults(
            string teamId,
            [FromQuery]string opponentTeamId = "",
            string language = "en-US")
            => await mediator.Send(new TeamResultRequest(teamId, opponentTeamId, language));

        [HttpGet]
        [Route("trending")]
        public async Task<IEnumerable<TeamProfile>> GetTrendingTeams(
            string language = "en-US")
            => await mediator.Send(new TrendingTeamsRequest(language));

        [HttpGet]
        [Route("search")]
        public async Task<IEnumerable<TeamProfile>> GetTrendingTeams(
            string keyword,
            string language = "en-US")
            => await mediator.Send(new SearchTeamByNameRequest(keyword, language));

        [HttpGet]
        [Route("{teamId}/matches")]
        public async Task<IEnumerable<MatchSummary>> GetMatches(
            string teamId,
            string language = "en-US")
            => await mediator.Send(new MatchesByTeamRequest(teamId, language));
    }
}