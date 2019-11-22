using MessagePack;
using Score247.Shared.Enumerations;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject]
    public class TeamOutcome : Enumeration
    {
        public static readonly TeamOutcome AFCChampionsLeague = new TeamOutcome(1, "AFC Champions League");
        public static readonly TeamOutcome AFCCup = new TeamOutcome(2, "AFC Cup");
        public static readonly TeamOutcome CAFConfederationCup = new TeamOutcome(3, "CAF Confederation Cup");
        public static readonly TeamOutcome ChampionsLeague = new TeamOutcome(4, "Champions League");
        public static readonly TeamOutcome ChampionsLeagueQualification = new TeamOutcome(5, "Champions League Qualification");
        public static readonly TeamOutcome ChampionsRound = new TeamOutcome(6, "Champions Round");
        public static readonly TeamOutcome ChampionshipRound = new TeamOutcome(7, "Championship Round");
        public static readonly TeamOutcome ClubChampionship = new TeamOutcome(8, "Club Championship");
        public static readonly TeamOutcome CopaLibertadores = new TeamOutcome(9, "Copa Libertadores");
        public static readonly TeamOutcome CopaLibertadoresQualification = new TeamOutcome(10, "Copa Libertadores Qualification");
        public static readonly TeamOutcome CopaSudamericana = new TeamOutcome(11, "Copa Sudamericana");
        public static readonly TeamOutcome CupWinners = new TeamOutcome(12, "Cup Winners");
        public static readonly TeamOutcome Eliminated = new TeamOutcome(13, "Eliminated");
        public static readonly TeamOutcome EuropaLeague = new TeamOutcome(14, "Europa League");
        public static readonly TeamOutcome EuropaLeagueQualification = new TeamOutcome(15, "Europa League Qualification");
        public static readonly TeamOutcome EuropeanCup = new TeamOutcome(16, "European Cup");
        public static readonly TeamOutcome FinalFour = new TeamOutcome(17, "Final Four");
        public static readonly TeamOutcome FinalRound = new TeamOutcome(18, "Final Round");
        public static readonly TeamOutcome Finals = new TeamOutcome(19, "Finals");
        public static readonly TeamOutcome GroupMatches = new TeamOutcome(20, "Group Matches");
        public static readonly TeamOutcome InternationalCompetition = new TeamOutcome(21, "International Competition");
        public static readonly TeamOutcome MainRound = new TeamOutcome(22, "Main Round");
        public static readonly TeamOutcome NextGroupPhase = new TeamOutcome(23, "Next Group Phase");
        public static readonly TeamOutcome PlacementMatches = new TeamOutcome(24, "Placement Matches");
        public static readonly TeamOutcome Playoffs = new TeamOutcome(25, "Playoffs");
        public static readonly TeamOutcome PreliminaryRound = new TeamOutcome(26, "Preliminary Round");
        public static readonly TeamOutcome Promotion = new TeamOutcome(27, "Promotion");
        public static readonly TeamOutcome PromotionPlayoff = new TeamOutcome(28, "Promotion Playoff");
        public static readonly TeamOutcome PromotionPlayoffs = new TeamOutcome(29, "Promotion Playoffs");
        public static readonly TeamOutcome PromotionRound = new TeamOutcome(30, "Promotion Round");
        public static readonly TeamOutcome QualificationPlayoffs = new TeamOutcome(31, "Qualification Playoffs");
        public static readonly TeamOutcome Qualified = new TeamOutcome(32, "Qualified");
        public static readonly TeamOutcome QualifyingRound = new TeamOutcome(33, "Qualifying Round");
        public static readonly TeamOutcome Relegation = new TeamOutcome(34, "Relegation");
        public static readonly TeamOutcome RelegationPlayoff = new TeamOutcome(35, "Relegation Playoff");
        public static readonly TeamOutcome RelegationPlayoffs = new TeamOutcome(36, "Relegation Playoffs");
        public static readonly TeamOutcome RelegationRound = new TeamOutcome(37, "Relegation Round");
        public static readonly TeamOutcome Semifinal = new TeamOutcome(38, "Semifinal");
        public static readonly TeamOutcome TopSix = new TeamOutcome(39, "Top Six");
        public static readonly TeamOutcome UEFACup = new TeamOutcome(40, "UEFA Cup");
        public static readonly TeamOutcome UEFACupQualification = new TeamOutcome(41, "UEFA Cup Qualification");
        public static readonly TeamOutcome UEFAIntertotoCup = new TeamOutcome(42, "UEFA Intertoto Cup");
        public static readonly TeamOutcome Unknown = new TeamOutcome(43, "Unknown");

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