using Soccer.Core._Shared.Enumerations;

namespace Soccer.Core.Favorites
{
    public class Favorite
    {
        public Favorite(string id, byte type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }

        public byte Type { get; }
    }
}
