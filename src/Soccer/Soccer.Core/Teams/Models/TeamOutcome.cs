using System;
using MessagePack;
using Score247.Shared.Enumerations;

namespace Soccer.Core.Teams.Models
{
    [Serializable]
    [MessagePackObject]
    public class TeamOutcome : Enumeration
    {
        public static readonly TeamOutcome AFCChampionsLeague = new TeamOutcome(1, "afc champions league", "AFC Champions League");
        public static readonly TeamOutcome AFCCup = new TeamOutcome(2, "afc cup", "AFC Cup");
        public static readonly TeamOutcome CAFConfederationCup = new TeamOutcome(3, "caf confederation cup", "CAF Confederation Cup");
        public static readonly TeamOutcome ChampionsLeague = new TeamOutcome(4, "champions league", "Champions League");
        public static readonly TeamOutcome ChampionsLeagueQualification = new TeamOutcome(5, "champions league qualification", "Champions League Qualification");
        public static readonly TeamOutcome ChampionsRound = new TeamOutcome(6, "champions round", "Champions Round");
        public static readonly TeamOutcome ChampionshipRound = new TeamOutcome(7, "championship round", "Championship Round");
        public static readonly TeamOutcome ClubChampionship = new TeamOutcome(8, "club championship", "Club Championship");
        public static readonly TeamOutcome CopaLibertadores = new TeamOutcome(9, "copa libertadores", "Copa Libertadores");
        public static readonly TeamOutcome CopaLibertadoresQualification = new TeamOutcome(10, "copa libertadores qualification", "Copa Libertadores Qualification");
        public static readonly TeamOutcome CopaSudamericana = new TeamOutcome(11, "copa sudamericana", "Copa Sudamericana");
        public static readonly TeamOutcome CupWinners = new TeamOutcome(12, "cup winners", "Cup Winners");
        public static readonly TeamOutcome Eliminated = new TeamOutcome(13, "eliminated", "Eliminated");
        public static readonly TeamOutcome EuropaLeague = new TeamOutcome(14, "europa league", "Europa League");
        public static readonly TeamOutcome EuropaLeagueQualification = new TeamOutcome(15, "europa league qualification", "Europa League Qualification");
        public static readonly TeamOutcome EuropeanCup = new TeamOutcome(16, "european cup", "European Cup");
        public static readonly TeamOutcome FinalFour = new TeamOutcome(17, "final four", "Final Four");
        public static readonly TeamOutcome FinalRound = new TeamOutcome(18, "final round", "Final Round");
        public static readonly TeamOutcome Finals = new TeamOutcome(19, "finals", "Finals");
        public static readonly TeamOutcome GroupMatches = new TeamOutcome(20, "group matches", "Group Matches");
        public static readonly TeamOutcome InternationalCompetition = new TeamOutcome(21, "international competition", "International Competition");
        public static readonly TeamOutcome MainRound = new TeamOutcome(22, "main round", "Main Round");
        public static readonly TeamOutcome NextGroupPhase = new TeamOutcome(23, "next group phase", "Next Group Phase");
        public static readonly TeamOutcome PlacementMatches = new TeamOutcome(24, "placement matches", "Placement Matches");
        public static readonly TeamOutcome Playoffs = new TeamOutcome(25, "playoffs", "Playoffs");
        public static readonly TeamOutcome PreliminaryRound = new TeamOutcome(26, "preliminary round", "Preliminary Round");
        public static readonly TeamOutcome Promotion = new TeamOutcome(27, "promotion", "Promotion");
        public static readonly TeamOutcome PromotionPlayoff = new TeamOutcome(28, "promotion playoff", "Promotion Playoff");
        public static readonly TeamOutcome PromotionPlayoffs = new TeamOutcome(29, "promotion playoffs", "Promotion Playoffs");
        public static readonly TeamOutcome PromotionRound = new TeamOutcome(30, "promotion round", "Promotion Round");
        public static readonly TeamOutcome QualificationPlayoffs = new TeamOutcome(31, "qualification playoffs", "Qualification Playoffs");
        public static readonly TeamOutcome Qualified = new TeamOutcome(32, "qualified", "Qualified");
        public static readonly TeamOutcome QualifyingRound = new TeamOutcome(33, "qualifying round", "Qualifying Round");
        public static readonly TeamOutcome Relegation = new TeamOutcome(34, "relegation", "Relegation");
        public static readonly TeamOutcome RelegationPlayoff = new TeamOutcome(35, "relegation playoff", "Relegation Playoff");
        public static readonly TeamOutcome RelegationPlayoffs = new TeamOutcome(36, "relegation playoffs", "Relegation Playoffs");
        public static readonly TeamOutcome RelegationRound = new TeamOutcome(37, "relegation round", "Relegation Round");
        public static readonly TeamOutcome Semifinal = new TeamOutcome(38, "semifinal", "Semifinal");
        public static readonly TeamOutcome TopSix = new TeamOutcome(39, "top six", "Top Six");
        public static readonly TeamOutcome UEFACup = new TeamOutcome(40, "uefa cup", "UEFA Cup");
        public static readonly TeamOutcome UEFACupQualification = new TeamOutcome(41, "uefa cup qualification", "UEFA Cup Qualification");
        public static readonly TeamOutcome UEFAIntertotoCup = new TeamOutcome(42, "uefa intertoto cup", "UEFA Intertoto Cup");
        public static readonly TeamOutcome Unknown = new TeamOutcome(43, "unknown", "Unknown");
        public static readonly TeamOutcome CupWinner = new TeamOutcome(44, "cup winner", "Cup Winner");

        public TeamOutcome()
        {
        }

        public TeamOutcome(byte value, string displayName, string friendlyName)
            : base(value, displayName)
        {
            FriendlyName = friendlyName;
        }

        public TeamOutcome(byte value)
            : base(value, value.ToString())
        {
        }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(2)]
#pragma warning restore S109 // Magic numbers should not be used
        public string FriendlyName { get; private set; }
    }
}