namespace Soccer.Core.Matches.Events
{
    public interface ILiveMatchUpdatedEvent
    {
        string MatchId { get; }

        string Language { get; }
    }
}
