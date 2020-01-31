using System.Collections.Generic;
using MediatR;

namespace Soccer.API.Favorites.Requests
{
    public class GetUsersByFavoriteRequest : IRequest<IReadOnlyList<string>>
    {
        public GetUsersByFavoriteRequest(string id, byte favoriteType) 
        {
            Id = id;
            FavoriteType = favoriteType;
        }

        public string Id { get; }

        public byte FavoriteType { get; }
    }
}
