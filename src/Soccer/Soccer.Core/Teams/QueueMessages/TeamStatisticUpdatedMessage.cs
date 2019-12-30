namespace Soccer.Core.Teams.QueueMessages
{
    using System;
    using Soccer.Core.Teams.Models;

    public interface ITeamStatisticUpdatedMessage
    {
        string MatchId { get; }

        bool IsHome { get; }

        bool IsUpdateOnlyRedCard { get; }

        TeamStatistic TeamStatistic { get; }

        DateTimeOffset EventDate { get; }
    }

    public class TeamStatisticUpdatedMessage : ITeamStatisticUpdatedMessage
    {
        public TeamStatisticUpdatedMessage(
            string matchId, bool isHome,
            TeamStatistic teamStatistic,
            bool isUpdateOnlyRedCard = false,
            DateTimeOffset eventDate = default)
        {
            MatchId = matchId;
            IsHome = isHome;
            TeamStatistic = teamStatistic;
            IsUpdateOnlyRedCard = isUpdateOnlyRedCard;
            EventDate = eventDate;
        }

        public string MatchId { get; }

        public bool IsHome { get; }

        public TeamStatistic TeamStatistic { get; }

        public bool IsUpdateOnlyRedCard { get; }

        public DateTimeOffset EventDate { get; }
    }
}