namespace Soccer.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Soccer.API.Modules.Matches.Queries;
    using Soccer.Core.Domain.Matches.Models;

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
        /// <param name="tz">07:00</param>
        /// <param name="language"></param>
        [HttpGet]
        public async Task<IEnumerable<Match>> Get(
                DateTime fd,
                DateTime td,
                TimeSpan tz,
                string language = "en-US")
            => await mediator.Send(new GetMatchesByDateQuery(fd, td, language, tz));

        [HttpGet]
        [Route("{id}")]
        public async Task<Match> Get(string id, TimeSpan tz, string language = "en-US")
            => await mediator.Send(new GetMatchByIdQuery(id, tz, language));

        [HttpGet]
        [Route("live")]
        public async Task<IEnumerable<Match>> Get(TimeSpan tz, string language = "en-US")
          => await mediator.Send(new GetLiveMatchesQuery(tz, language));
    }
}