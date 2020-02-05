using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Score247.Shared.Enumerations;
using Soccer.API.Favorites.Models;
using Soccer.API.Favorites.Requests;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Models.Favorites;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Favorites
{
    [Route("api/soccer/{language}/favorites/")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IMediator mediator;

        public FavoriteController(IMediator mediator)
            => this.mediator = mediator;

        [HttpPost]
        [Route("add/")]
        public async Task<bool> Add([FromBody]UserFavorite userFavorite, string language = Language.English)
            => await mediator.Send(new AddFavoriteRequest(userFavorite));

        [HttpDelete]
        [Route("remove/")]
        public async Task<bool> Remove(string userId, string favoriteId, string language = Language.English)
            => await mediator.Send(new RemoveFavoriteRequest(userId, favoriteId));

        [HttpGet]
        [Route("users/get/")]
        public async Task<IReadOnlyList<string>> GetByMatchId(string id, byte favoriteType = FavoriteType.MatchValue, string language = Language.English)
            => await mediator.Send(new GetUsersByFavoriteRequest(id, favoriteType));

        [HttpPost]
        [Route("sync/")]
        public async Task<bool> Sync(
            [FromBody]SyncUserFavorite userFavorite, 
            string language = Language.English)
            => await mediator.Send(new SyncFavoriteRequest(userFavorite, Enumeration.FromDisplayName<Language>(language)));
    }
}