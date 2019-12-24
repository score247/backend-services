using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.Matches.Models;
using Soccer.API.Matches.Requests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches
{
    [Route("api/soccer/{language}/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMediator mediator;

        public MatchController(IMediator mediator)
            => this.mediator = mediator;

        /// <summary>
        /// Get Matches By Date Range
        /// </summary>
        /// <param name="fd">2019-07-15T00:00:00+07:00</param>
        /// <param name="td">2019-07-16T23:59:59+07:00</param>
        /// <param name="language"></param>
        [HttpGet]
        public async Task<IEnumerable<MatchSummary>> Get(
                DateTimeOffset fd,
                DateTimeOffset td,
                string language = Language.English)
            => await mediator.Send(new MatchesByDateRequest(fd, td, language));

        /// <summary>
        /// Get Match By Id
        /// </summary>
        /// <param name="id">sr:match:13635269</param>
        /// <param name="language"></param>
        /// <param name="eventDate">2019-07-16T23:59:59+07:00</param>
        [HttpGet]
        [Route("{id}")]
        public async Task<MatchInfo> Get(string id, string language = Language.English, DateTimeOffset eventDate = default)
            => await mediator.Send(new MatchInfoByIdRequest(id, language, eventDate));

        [HttpGet]
        [Route("live")]
        public async Task<IEnumerable<MatchSummary>> Get(string language = Language.English)
          => await mediator.Send(new LiveMatchesRequest(language));

        [HttpGet]
        [Route("live/count")]
        public async Task<int> GetLiveCount(string language = Language.English)
          => await mediator.Send(new LiveMatchCountRequest());

        [HttpGet]
        [Route("{id}/coverage")]
        public async Task<MatchCoverage> GetMatchCoverage(string id, string language = Language.English, DateTimeOffset eventDate = default)
           => await mediator.Send(new MatchCoverageByIdRequest(id, language, eventDate));

        [HttpGet]
        [Route("{id}/commentaries")]
        public async Task<IEnumerable<MatchCommentary>> GetMatchCommentaries(string id, string language = Language.English, DateTimeOffset eventDate = default)
           => await mediator.Send(new MatchCommentaryByIdRequest(id, language, eventDate));

        [HttpGet]
        [Route("{id}/statistic")]
        public async Task<MatchStatistic> GetMatchStatistic(string id, DateTimeOffset eventDate = default)
           => await mediator.Send(new MatchStatisticRequest(id, eventDate));

        [HttpGet]
        [Route("{id}/lineups")]
        public async Task<MatchPitchViewLineups> GetMatchLineups(string id, string language = Language.English, DateTimeOffset eventDate = default)
           => await mediator.Send(new MatchLineupsRequest(id, language, eventDate));

        [HttpPost]
        [Route("ids/")]
        public async Task<IEnumerable<MatchSummary>> GetByIds([FromBody]string[] ids, string language = Language.English)
            => await mediator.Send(new MatchesByIdsRequest(ids, language));
    }
}