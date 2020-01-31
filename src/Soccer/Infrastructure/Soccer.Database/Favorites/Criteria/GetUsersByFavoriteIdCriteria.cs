using Fanex.Data.Repository;
using Score247.Shared.Enumerations;

namespace Soccer.Database.Favorites.Criteria
{
    public class GetUsersByFavoriteIdCriteria : CriteriaBase
    {
        public GetUsersByFavoriteIdCriteria(string id, byte favoriteType) 
        {
            SportId = Sport.Soccer.Value;
            Id = id;
            FavoriteType = favoriteType;
        }

        public int SportId { get; }

        public string Id { get; }

        public byte FavoriteType { get; }

        public override string GetSettingKey() => "UserFavorite_GetUsersByFavoriteId";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Id) && FavoriteType > 0;
    }
}
