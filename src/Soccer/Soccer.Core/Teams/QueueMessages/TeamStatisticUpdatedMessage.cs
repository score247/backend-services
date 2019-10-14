namespace Soccer.Core.Teams.QueueMessages
{
    using Soccer.Core.Teams.Models;

    public interface ITeamStatisticUpdatedMessage
    {
        string MatchId { get; }

        bool IsHome { get; }

        bool IsUpdateOnlyRedCard { get; }

        TeamStatistic TeamStatistic { get; }
    }

    public class TeamStatisticUpdatedMessage : ITeamStatisticUpdatedMessage
    {
        public TeamStatisticUpdatedMessage(string matchId, bool isHome, TeamStatistic teamStatistic, bool isUpdateOnlyRedCard = false)
        {
            MatchId = matchId;
            IsHome = isHome;
            TeamStatistic = teamStatistic;
            IsUpdateOnlyRedCard = isUpdateOnlyRedCard;
        }

        public string MatchId { get; }

        public bool IsHome { get; }

        public TeamStatistic TeamStatistic { get; }

        public bool IsUpdateOnlyRedCard { get; }
    }
}