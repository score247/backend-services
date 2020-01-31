using Score247.Shared.Enumerations;

namespace Soccer.Database.Favorites.Commands
{
    public class RemoveFavoriteCommand : BaseCommand
    {
        public RemoveFavoriteCommand(string userId, string favoriteId)
        {
            SportId = Sport.Soccer.Value;
            UserId = userId;
            FavoriteId = favoriteId;
        }

        public int SportId { get; }

        public string UserId { get; }

        public string FavoriteId { get; }

        public override string GetSettingKey() => "UserFavorite_RemoveById";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(FavoriteId);
    }
}
