using Score247.Shared.Enumerations;

namespace Soccer.Core._Shared.Enumerations
{
    public class NotificationType : Enumeration
    {
        public const byte MatchValue = 1;
        public const byte LeagueValue = 2;

        public static readonly NotificationType Match = new NotificationType(MatchValue, "match");
        public static readonly NotificationType League = new NotificationType(LeagueValue, "league");

        public NotificationType()
        {
        }

        public NotificationType(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
