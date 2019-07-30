namespace Soccer.Core.Matches.Events
{
    using Soccer.Core._Shared.Enumerations;

    public interface ILiveMatchUpdatedEvent
    {
        string MatchId { get; }

        string Language { get; }
    }

    public class LiveMatchUpdatedEvent : ILiveMatchUpdatedEvent
    {
        public LiveMatchUpdatedEvent(string matchId, Language language)
        {
            MatchId = matchId;
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Language { get; }
    }
}