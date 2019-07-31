namespace Soccer.Core.Matches.Events
{
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