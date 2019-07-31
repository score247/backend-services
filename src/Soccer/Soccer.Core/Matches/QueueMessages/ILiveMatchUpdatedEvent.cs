namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Shared.Enumerations;

    public interface ILiveMatchUpdatedEvent
    {
        string MatchId { get; }
    }

    public class LiveMatchUpdatedEvent : ILiveMatchUpdatedEvent
    {
        public LiveMatchUpdatedEvent(string matchId)
        {
            MatchId = matchId;            
        }

        public string MatchId { get; }

        public string Language { get; }
    }
}