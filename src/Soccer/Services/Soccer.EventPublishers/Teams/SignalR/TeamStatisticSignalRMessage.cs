namespace Soccer.EventPublishers.Teams.SignalR
{
    using Soccer.Core.Teams.Models;

    internal class TeamStatisticSignalRMessage
    {
        public TeamStatisticSignalRMessage(
            byte sportId,
            string matchId,
            bool isHome,
            TeamStatistic teamStatistic)
        {
            SportId = sportId;
            MatchId = matchId;
            IsHome = isHome;
            TeamStatistic = teamStatistic;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public bool IsHome { get; }

        public TeamStatistic TeamStatistic { get; }
    }
}