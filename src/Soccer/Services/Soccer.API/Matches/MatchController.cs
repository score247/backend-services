using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.Matches.Models;
using Soccer.API.Matches.Requests;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Matches
{
    [Route("api/soccer/{language}/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private const string englishCode = "en-US";
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
                DateTime fd,
                DateTime td,
                string language = englishCode)
            => await mediator.Send(new MatchesByDateRequest(fd, td, language));

        [HttpGet]
        [Route("{id}")]
        public async Task<MatchInfo> Get(string id, string language = englishCode)
            => await mediator.Send(new MatchInfoByIdRequest(id, language));

        [HttpGet]
        [Route("live")]
        public async Task<IEnumerable<MatchSummary>> Get(string language = englishCode)
          => await mediator.Send(new LiveMatchesRequest(language));

        [HttpGet]
        [Route("{id}/coverage")]
        public async Task<MatchCoverage> GetMatchCoverage(string id, string language = englishCode)
           => await mediator.Send(new MatchCoverageByIdRequest(id, language));

        [HttpGet]
        [Route("{id}/commentaries")]
        public async Task<IEnumerable<MatchCommentary>> GetMatchCommentaries(string id, string language = englishCode)
           => await mediator.Send(new MatchCommentaryByIdRequest(id, language));

        [HttpGet]
        [Route("{id}/statistic")]
        public async Task<MatchStatistic> GetMatchStatistic(string id)
           => await mediator.Send(new MatchStatisticRequest(id));

        [HttpGet]
        [Route("{id}/lineups")]
        public async Task<MatchPitchViewLineups> GetMatchLineups(string id, string language = englishCode)
           => await mediator.Send(new MatchLineupsRequest(id, language));
    }
}