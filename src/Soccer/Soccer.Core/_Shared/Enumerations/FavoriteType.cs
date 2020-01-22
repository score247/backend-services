using Score247.Shared.Enumerations;

namespace Soccer.Core._Shared.Enumerations
{
    public class FavoriteType : Enumeration
    {
        public static readonly FavoriteType Match = new FavoriteType(1, "match");
        public static readonly FavoriteType League = new FavoriteType(2, "league");
        public static readonly FavoriteType Team = new FavoriteType(3, "team");

        public FavoriteType()
        {
        }

        public FavoriteType(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
