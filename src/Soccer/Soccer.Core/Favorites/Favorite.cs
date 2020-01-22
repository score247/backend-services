using Soccer.Core._Shared.Enumerations;

namespace Soccer.Core.Favorites
{
    public class Favorite
    {
        public Favorite(string id, FavoriteType type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }

        public FavoriteType Type { get; }
    }
}
