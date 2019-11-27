namespace Soccer.Core.Matches.QueueMessages
{
    using System;
    using Soccer.Core.Matches.Models;

    public interface IMatchUpdatedCoverageInfo
    {
        string MatchId { get; }

        Coverage Coverage { get; }

        DateTimeOffset EventDate { get; }
    }

    public class MatchUpdatedCoverageInfo : IMatchUpdatedCoverageInfo
    {
        public MatchUpdatedCoverageInfo(string matchId, Coverage coverage, DateTimeOffset eventDate = default)
        {
            MatchId = matchId;
            Coverage = coverage;
            EventDate = eventDate;
        }

        public string MatchId { get; }

        public Coverage Coverage { get; }

        public DateTimeOffset EventDate { get; }
    }
}
