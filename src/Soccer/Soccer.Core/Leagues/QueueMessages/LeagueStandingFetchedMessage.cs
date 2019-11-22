using Soccer.Core.Leagues.Models;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueStandingFetchedMessage
    {
        LeagueTable LeagueStanding { get; }
        string Language { get; }
    }

    public class LeagueStandingFetchedMessage : ILeagueStandingFetchedMessage
    {
        public LeagueTable LeagueStanding { get; }
        public string Language { get; }

        public LeagueStandingFetchedMessage(LeagueTable leagueStanding, string language)
        {
            LeagueStanding = leagueStanding;
            Language = language;
        }
    }
}