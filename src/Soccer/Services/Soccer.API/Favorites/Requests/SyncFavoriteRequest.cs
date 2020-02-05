using MediatR;
using Soccer.API.Favorites.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Favorites.Requests
{
    public class SyncFavoriteRequest : IRequest<bool>
    {
        public SyncFavoriteRequest(SyncUserFavorite syncUserFavorite, Language language)
        {
            SyncUserFavorite = syncUserFavorite;
            Language = language;
        }

        public SyncUserFavorite SyncUserFavorite { get; }

        public Language Language { get; }
    }
}
