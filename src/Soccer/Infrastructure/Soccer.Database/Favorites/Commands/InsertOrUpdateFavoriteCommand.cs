using Score247.Shared.Enumerations;
using Soccer.Core.Favorites;

namespace Soccer.Database.Favorites.Commands
{
    public class InsertOrUpdateFavoriteCommand : BaseCommand
    {
        public InsertOrUpdateFavoriteCommand(UserFavorite userFavorite)
        {
            SportId = Sport.Soccer.Value;
            UserId = userFavorite.UserId;
            Favorites = ToJsonString(userFavorite.Favorites);
        }

        public int SportId { get; }

        public string UserId { get; }

        public string Favorites { get; }

        public override string GetSettingKey() => "UserFavorite_InsertOrUpdate";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Favorites);
    }
}
