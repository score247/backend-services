namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public interface ILiveMatchClosedMessage
    {
        string MatchId { get; }

        string Language { get; }

        MatchResult MatchResult { get; }
    }

    public class LiveMatchClosedMessage : ILiveMatchClosedMessage
    {
        public LiveMatchClosedMessage(string matchId, Language language, MatchResult matchResult)
        {
            MatchId = matchId;
            Language = language.DisplayName;
            MatchResult = matchResult;
        }

        public string MatchId { get; }

        public string Language { get; }

        public MatchResult MatchResult { get; }
    }
}