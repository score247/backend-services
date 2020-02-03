using MessagePack;
using Score247.Shared.Enumerations;

namespace Soccer.Core._Shared.Enumerations
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class FavoriteType : Enumeration
    {
        public const byte MatchValue = 1;
        public const byte LeagueValue = 2;
        public const byte TeamValue = 3;

        public static readonly FavoriteType Match = new FavoriteType(MatchValue, "match");
        public static readonly FavoriteType League = new FavoriteType(LeagueValue, "league");
        public static readonly FavoriteType Team = new FavoriteType(TeamValue, "team");

        public FavoriteType()
        {
        }

        public FavoriteType(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
