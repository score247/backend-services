using Soccer.Core.Models.Favorites;

namespace Soccer.API.Favorites.Models
{
    public class SyncUserFavorite
    {
        public SyncUserFavorite(UserFavorite addedUserFavorite, UserFavorite removedUserFavorite)
        {
            AddedUserFavorite = addedUserFavorite;
            RemovedUserFavorite = removedUserFavorite;
        }

        public UserFavorite AddedUserFavorite { get; }

        public UserFavorite RemovedUserFavorite { get; }
    }
}
