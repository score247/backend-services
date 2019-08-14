namespace Soccer.Core.Teams.QueueMessages
{
    using Soccer.Core.Teams.Models;

    public interface ITeamStatisticUpdatedMessage
    {
        string MatchId { get; }

        bool IsHome { get; }

        TeamStatistic TeamStatistic { get; }
    }

    public class TeamStatisticUpdatedMessage : ITeamStatisticUpdatedMessage
    {
        public TeamStatisticUpdatedMessage(string matchId, bool isHome, TeamStatistic teamStatistic)
        {
            MatchId = matchId;
            IsHome = isHome;
            TeamStatistic = teamStatistic;
        }

        public string MatchId { get; }

        public bool IsHome { get; }

        public TeamStatistic TeamStatistic { get; }
    }
}