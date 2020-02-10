using Fanex.Data.Repository;
using Score247.Shared.Enumerations;

namespace Soccer.Database.Favorites.Criteria
{
    public class GetUsersByFavoriteIdCriteria : CriteriaBase
    {
        public GetUsersByFavoriteIdCriteria(string matchId) 
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
        }

        public int SportId { get; }

        public string MatchId { get; }

        public override string GetSettingKey() => "UserFavorite_GetUsersByFavoriteId";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId);
    }
}
