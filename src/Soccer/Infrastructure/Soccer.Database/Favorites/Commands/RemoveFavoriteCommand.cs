using Score247.Shared.Enumerations;
using Soccer.Core.Models.Favorites;

namespace Soccer.Database.Favorites.Commands
{
    public class RemoveFavoriteCommand : BaseCommand
    {
        public RemoveFavoriteCommand(UserFavorite userFavorite)
        {
            SportId = Sport.Soccer.Value;
            UserId = userFavorite.UserId;
            Favorites = ToJsonString(userFavorite.Favorites);
        }

        public int SportId { get; }

        public string UserId { get; }

        public string Favorites { get; }

        public override string GetSettingKey() => "UserFavorite_RemoveById";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Favorites);
    }
}
