namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;

    public interface ILiveMatchResultUpdatedMessage
    {
        string MatchId { get; }

        MatchResult MatchResult { get; }
    }

    public class LiveMatchResultUpdatedMessage : ILiveMatchResultUpdatedMessage
    {
        public LiveMatchResultUpdatedMessage(string matchId, MatchResult matchResult)
        {
            MatchId = matchId;
            MatchResult = matchResult;
        }

        public string MatchId { get; }

        public MatchResult MatchResult { get; }
    }
}