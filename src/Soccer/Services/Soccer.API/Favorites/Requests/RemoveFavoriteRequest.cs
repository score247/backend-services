using MediatR;
using Soccer.Core.Models.Favorites;

namespace Soccer.API.Favorites.Requests
{
    public class RemoveFavoriteRequest : IRequest<bool>
    {
        public RemoveFavoriteRequest(UserFavorite userFavorite)
        {
            UserFavorite = userFavorite;
        }

        public UserFavorite UserFavorite { get; }
    }
}
