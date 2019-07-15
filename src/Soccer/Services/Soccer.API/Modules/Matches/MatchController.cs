namespace Soccer.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Soccer.API.Modules.Matches.Queries;
    using Soccer.Core.Domain.Matches;

    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMediator mediator;

        public MatchController(IMediator mediator)
            => this.mediator = mediator;

        public async Task<IEnumerable<Match>> Get(
            int sportId,
            DateTime from,
            DateTime to,
            string language,
            TimeSpan timeSpan)
            => await mediator.Send(new GetMatchesByDateQuery(sportId, from, to, language, timeSpan));
    }
}