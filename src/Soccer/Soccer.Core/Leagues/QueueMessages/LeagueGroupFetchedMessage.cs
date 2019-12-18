using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueGroupFetchedMessage 
    {
        string LeagueId { get; }

        string LeagueGroupName { get; }

        Language Language { get; }
    }

    public class LeagueGroupFetchedMessage : ILeagueGroupFetchedMessage
    {
        public LeagueGroupFetchedMessage(string leagueId, string leagueGroupName, Language language) 
        {
            LeagueId = leagueId;
            LeagueGroupName = leagueGroupName;
            Language = language;
        }

        public string LeagueId { get; }

        public string LeagueGroupName { get; }

        public Language Language { get; }
    }
}
