using System.Collections.Generic;

namespace Soccer.Core.Favorites
{
    public class UserFavorite
    {
        public UserFavorite(string userId, IList<Favorite> favorites)
        {
            UserId = userId;
            Favorites = favorites;
        }

        public string UserId { get; }

        public IList<Favorite> Favorites { get; }
    }
}
