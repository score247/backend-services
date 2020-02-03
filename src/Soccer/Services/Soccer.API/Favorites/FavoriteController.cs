﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.Favorites.Requests;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Models.Favorites;

namespace Soccer.API.Favorites
{
    [Route("api/soccer/favorites/")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IMediator mediator;

        public FavoriteController(IMediator mediator)
            => this.mediator = mediator;

        [HttpPost]
        [Route("add/")]
        public async Task<bool> Add([FromBody]UserFavorite userFavorite)
            => await mediator.Send(new AddFavoriteRequest(userFavorite));

        [HttpDelete]
        [Route("remove/")]
        public async Task<bool> Remove(string userId, string favoriteId)
            => await mediator.Send(new RemoveFavoriteRequest(userId, favoriteId));

        [HttpGet]
        [Route("users/get/")]
        public async Task<IReadOnlyList<string>> GetByMatchId(string id, byte favoriteType = FavoriteType.MatchValue)
            => await mediator.Send(new GetUsersByFavoriteRequest(id, favoriteType));
    }
}