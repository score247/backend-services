namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;

    public interface ILiveMatchUpdatedMessage
    {
        string MatchId { get; }

        MatchResult MatchResult { get; }
    }

    public class LiveMatchUpdatedMessage : ILiveMatchUpdatedMessage
    {
        public LiveMatchUpdatedMessage(string matchId, MatchResult matchResult)
        {
            MatchId = matchId;
            MatchResult = matchResult;
        }

        public string MatchId { get; }

        public MatchResult MatchResult { get; }
    }
}