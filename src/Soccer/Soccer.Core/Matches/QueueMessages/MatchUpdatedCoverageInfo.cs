namespace Soccer.Core.Matches.QueueMessages
{
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public interface IMatchUpdatedCoverageInfo
    {
        string MatchId { get; }

        Language Language { get; }

        Coverage Coverage { get; }
    }

    public class MatchUpdatedCoverageInfo : IMatchUpdatedCoverageInfo
    {
        public MatchUpdatedCoverageInfo(string matchId, Language language, Coverage coverage)
        {
            MatchId = matchId;
            Language = language;
            Coverage = coverage;
        }

        public string MatchId { get; }

        public Language Language { get; }

        public Coverage Coverage { get; }
    }
}
