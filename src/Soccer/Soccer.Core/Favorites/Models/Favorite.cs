using MessagePack;

namespace Soccer.Core.Models.Favorites
{
    [MessagePackObject(keyAsPropertyName: true)]
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
