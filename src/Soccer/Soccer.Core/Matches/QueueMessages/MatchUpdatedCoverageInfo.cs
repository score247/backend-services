namespace Soccer.Core.Matches.QueueMessages
{
    using Soccer.Core.Matches.Models;

    public interface IMatchUpdatedCoverageInfo
    {
        string MatchId { get; }

        Coverage Coverage { get; }
    }

    public class MatchUpdatedCoverageInfo : IMatchUpdatedCoverageInfo
    {
        public MatchUpdatedCoverageInfo(string matchId, Coverage coverage)
        {
            MatchId = matchId;
            Coverage = coverage;
        }

        public string MatchId { get; }

        public Coverage Coverage { get; }
    }
}
