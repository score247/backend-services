using MediatR;
using Soccer.Core.Favorites;

namespace Soccer.API.Favorites.Requests
{
    public class AddFavoriteRequest : IRequest<bool>
    {
        public AddFavoriteRequest(UserFavorite userFavorite)
        {
            UserFavorite = userFavorite;
        }

        public UserFavorite UserFavorite { get; }
    }
}
