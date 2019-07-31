namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;

    public interface ILiveMatchClosedMessage
    {
        string MatchId { get; }

        MatchResult MatchResult { get; }
    }

    public class LiveMatchClosedMessage : ILiveMatchClosedMessage
    {
        public LiveMatchClosedMessage(string matchId, MatchResult matchResult)
        {
            MatchId = matchId;
            MatchResult = matchResult;
        }

        public string MatchId { get; }

        public MatchResult MatchResult { get; }
    }
}