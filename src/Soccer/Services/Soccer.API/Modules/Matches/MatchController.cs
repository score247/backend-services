namespace Soccer.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Soccer.API.Modules.Matches.Queries;
    using Soccer.Core.Domain.Matches.Models;

    [Route("api/{language}/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMediator mediator;

        public MatchController(IMediator mediator)
            => this.mediator = mediator;

        /// <summary>
        /// Get Matches By Date Range
        /// </summary>
        /// <param name="sportId">Soccer (1)</param>
        /// <param name="fd">2019-07-15T00:00:00+07:00</param>
        /// <param name="td">2019-07-16T23:59:59+07:00</param>
        /// <param name="tz">07:00</param>
        /// <param name="language"></param>
        [HttpGet]
        public async Task<IEnumerable<Match>> Get(
                int sportId,
                DateTime fd,
                DateTime td,
                TimeSpan tz,
                string language = "en-US")
            => await mediator.Send(new GetMatchesByDateQuery(sportId, fd, td, language, tz));
    }
}