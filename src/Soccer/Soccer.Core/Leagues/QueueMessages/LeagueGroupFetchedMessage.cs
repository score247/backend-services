using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueGroupFetchedMessage
    {
        string LeagueId { get; }

        string LeagueSeasonId { get; }

        string LeagueGroupName { get; }

        LeagueRound LeagueRound { get; }

        bool HasGroup { get; }

        Language Language { get; }
    }

    public class LeagueGroupFetchedMessage : ILeagueGroupFetchedMessage
    {
        public LeagueGroupFetchedMessage(string leagueId, string leagueSeasonId, string leagueGroupName, LeagueRound leagueRound, bool hasGroup, Language language)
        {
            LeagueId = leagueId;
            LeagueSeasonId = leagueSeasonId;
            LeagueGroupName = leagueGroupName;
            LeagueRound = leagueRound;
            HasGroup = hasGroup;
            Language = language;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string LeagueGroupName { get; }

        public LeagueRound LeagueRound { get; }

        public bool HasGroup { get; }

        public Language Language { get; }
    }
}