using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.News.Requests;
using Soccer.Core.News.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.News
{
    [Route("api/soccer/{language}/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IMediator mediator;

        public NewsController(IMediator mediator)
            => this.mediator = mediator;

        /// <summary>
        /// Get News
        /// </summary>       
        /// <param name="language"></param>
        [HttpGet]
        public async Task<IEnumerable<NewsItem>> Get(
                string language = Language.English)
            => await mediator.Send(new NewsRequest(language));
    }
}
