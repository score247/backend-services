﻿namespace Soccer.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Soccer.API.Matches.Requests;
    using Soccer.Core.Matches.Models;

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
            => await mediator.Send(new MatchesByDateRequest(fd, td, language, tz));

        [HttpGet]
        [Route("{id}")]
        public async Task<Match> Get(string id, string language = "en-US")
            => await mediator.Send(new MatchByIdRequest(id, language));

        [HttpGet]
        [Route("live")]
        public async Task<IEnumerable<Match>> Get(TimeSpan tz, string language = "en-US")
          => await mediator.Send(new LiveMatchesRequest(tz, language));
    }
}