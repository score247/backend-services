namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public interface ILiveMatchUpdatedToClosedEvent
    {
        string MatchId { get; }

        string Language { get; }

        MatchResult MatchResult { get; }
    }

    public class LiveMatchUpdatedToClosedEvent : ILiveMatchUpdatedToClosedEvent
    {
        public LiveMatchUpdatedToClosedEvent(string matchId, Language language, MatchResult matchResult)
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