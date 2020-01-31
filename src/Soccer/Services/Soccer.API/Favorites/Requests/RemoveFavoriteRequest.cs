using MediatR;

namespace Soccer.API.Favorites.Requests
{
    public class RemoveFavoriteRequest : IRequest<bool>
    {
        public RemoveFavoriteRequest(string userId, string favoriteId)
        {
            UserId = userId;
            FavoriteId = favoriteId;
        }

        public string UserId { get; }

        public string FavoriteId { get; }
    }
}
