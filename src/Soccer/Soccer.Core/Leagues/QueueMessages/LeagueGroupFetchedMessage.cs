using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueGroupFetchedMessage
    {
        string LeagueId { get; }

        string LeagueSeasonId { get; }

        string LeagueGroupName { get; }

        string GroupName { get; }

        LeagueRound LeagueRound { get; }

        Language Language { get; }
    }

    public class LeagueGroupFetchedMessage : ILeagueGroupFetchedMessage
    {
        public LeagueGroupFetchedMessage(string leagueId, string leagueSeasonId, string leagueGroupName, string groupName, LeagueRound leagueRound, Language language)
        {
            LeagueId = leagueId;
            LeagueSeasonId = leagueSeasonId;
            LeagueGroupName = leagueGroupName;
            GroupName = groupName;
            LeagueRound = leagueRound;
            Language = language;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string LeagueGroupName { get; }

        public string GroupName { get; }

        public LeagueRound LeagueRound { get; }

        public Language Language { get; }
    }
}