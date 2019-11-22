using MessagePack;
using Score247.Shared.Enumerations;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject]
    public class TeamOutcome : Enumeration
    {
        public static readonly TeamOutcome AFCChampionsLeague = new TeamOutcome(1, "afc champions league");
        public static readonly TeamOutcome AFCCup = new TeamOutcome(2, "afc cup");
        public static readonly TeamOutcome CAFConfederationCup = new TeamOutcome(3, "caf confederation cup");
        public static readonly TeamOutcome ChampionsLeague = new TeamOutcome(4, "champions league");
        public static readonly TeamOutcome ChampionsLeagueQualification = new TeamOutcome(5, "champions league qualification");
        public static readonly TeamOutcome ChampionsRound = new TeamOutcome(6, "champions round");
        public static readonly TeamOutcome ChampionshipRound = new TeamOutcome(7, "championship round");
        public static readonly TeamOutcome ClubChampionship = new TeamOutcome(8, "club championship");
        public static readonly TeamOutcome CopaLibertadores = new TeamOutcome(9, "copa libertadores");
        public static readonly TeamOutcome CopaLibertadoresQualification = new TeamOutcome(10, "copa libertadores qualification");
        public static readonly TeamOutcome CopaSudamericana = new TeamOutcome(11, "copa sudamericana");
        public static readonly TeamOutcome CupWinners = new TeamOutcome(12, "cup winners");
        public static readonly TeamOutcome Eliminated = new TeamOutcome(13, "eliminated");
        public static readonly TeamOutcome EuropaLeague = new TeamOutcome(14, "europa league");
        public static readonly TeamOutcome EuropaLeagueQualification = new TeamOutcome(15, "europa league qualification");
        public static readonly TeamOutcome EuropeanCup = new TeamOutcome(16, "european cup");
        public static readonly TeamOutcome FinalFour = new TeamOutcome(17, "final four");
        public static readonly TeamOutcome FinalRound = new TeamOutcome(18, "final round");
        public static readonly TeamOutcome Finals = new TeamOutcome(19, "finals");
        public static readonly TeamOutcome GroupMatches = new TeamOutcome(20, "group matches");
        public static readonly TeamOutcome InternationalCompetition = new TeamOutcome(21, "international competition");
        public static readonly TeamOutcome MainRound = new TeamOutcome(22, "main round");
        public static readonly TeamOutcome NextGroupPhase = new TeamOutcome(23, "next group phase");
        public static readonly TeamOutcome PlacementMatches = new TeamOutcome(24, "placement matches");
        public static readonly TeamOutcome Playoffs = new TeamOutcome(25, "playoffs");
        public static readonly TeamOutcome PreliminaryRound = new TeamOutcome(26, "preliminary round");
        public static readonly TeamOutcome Promotion = new TeamOutcome(27, "promotion");
        public static readonly TeamOutcome PromotionPlayoff = new TeamOutcome(28, "promotion playoff");
        public static readonly TeamOutcome PromotionPlayoffs = new TeamOutcome(29, "promotion playoffs");
        public static readonly TeamOutcome PromotionRound = new TeamOutcome(30, "promotion round");
        public static readonly TeamOutcome QualificationPlayoffs = new TeamOutcome(31, "qualification playoffs");
        public static readonly TeamOutcome Qualified = new TeamOutcome(32, "qualified");
        public static readonly TeamOutcome QualifyingRound = new TeamOutcome(33, "qualifying round");
        public static readonly TeamOutcome Relegation = new TeamOutcome(34, "relegation");
        public static readonly TeamOutcome RelegationPlayoff = new TeamOutcome(35, "relegation playoff");
        public static readonly TeamOutcome RelegationPlayoffs = new TeamOutcome(36, "relegation playoffs");
        public static readonly TeamOutcome RelegationRound = new TeamOutcome(37, "relegation round");
        public static readonly TeamOutcome Semifinal = new TeamOutcome(38, "semifinal");
        public static readonly TeamOutcome TopSix = new TeamOutcome(39, "top six");
        public static readonly TeamOutcome UEFACup = new TeamOutcome(40, "uefa cup");
        public static readonly TeamOutcome UEFACupQualification = new TeamOutcome(41, "uefa cup qualification");
        public static readonly TeamOutcome UEFAIntertotoCup = new TeamOutcome(42, "uefa intertoto Cup");
        public static readonly TeamOutcome Unknown = new TeamOutcome(43, "unknown");

        public TeamOutcome()
        {
        }

        public TeamOutcome(byte value, string displayName)
            : base(value, displayName)
        {
        }

        public TeamOutcome(byte value)
            : base(value, value.ToString())
        {
        }
    }
}