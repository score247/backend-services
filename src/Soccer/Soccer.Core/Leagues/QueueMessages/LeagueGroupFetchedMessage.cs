using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueGroupFetchedMessage
    {
        string LeagueId { get; }

        string LeagueGroupName { get; }

        string GroupName { get; }

        Language Language { get; }
    }

    public class LeagueGroupFetchedMessage : ILeagueGroupFetchedMessage
    {
        public LeagueGroupFetchedMessage(string leagueId, string leagueGroupName, string groupName, Language language)
        {
            LeagueId = leagueId;
            LeagueGroupName = leagueGroupName;
            GroupName = groupName;
            Language = language;
        }

        public string LeagueId { get; }

        public string LeagueGroupName { get; }

        public string GroupName { get; }

        public Language Language { get; }
    }
}