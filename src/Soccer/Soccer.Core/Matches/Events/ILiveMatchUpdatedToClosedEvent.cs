namespace Soccer.Core.Matches.Events
{
    public interface ILiveMatchUpdatedToClosedEvent
    {
        string MatchId { get; }

        string MatchStatus { get; }

        string EventStatus { get; }

        string Language { get; }
    }
}
