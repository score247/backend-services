using MessagePack;
using Score247.Shared.Enumerations;

namespace Soccer.Core._Shared.Enumerations
{
    [MessagePackObject]
    public class LeagueTableType : Enumeration
    {
        public const string Total = "total";
        public const string Home = "home";
        public const string Away = "away";

        public static readonly LeagueTableType TotalTable = new LeagueTableType(1, Total);
        public static readonly LeagueTableType HomeTable = new LeagueTableType(2, Home);
        public static readonly LeagueTableType AwayTable = new LeagueTableType(3, Away);

        public LeagueTableType()
        {
        }

        public LeagueTableType(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}