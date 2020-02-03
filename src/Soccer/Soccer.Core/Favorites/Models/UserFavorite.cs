using System.Collections.Generic;
using MessagePack;

namespace Soccer.Core.Models.Favorites
{
    [MessagePackObject(keyAsPropertyName: true)]
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
