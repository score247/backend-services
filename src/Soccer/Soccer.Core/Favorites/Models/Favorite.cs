using MessagePack;

namespace Soccer.Core.Models.Favorites
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Favorite
    {
        public Favorite(string id, byte type, string group = null)
        {
            Id = id;
            Type = type;
            Group = group;
        }

        public string Id { get; }

        public byte Type { get; }

        public string Group { get; }
    }
}
