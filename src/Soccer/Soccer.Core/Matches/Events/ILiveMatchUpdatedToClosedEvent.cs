namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;

    public interface ILiveMatchUpdatedToClosedEvent
    {
        string MatchId { get; }

        MatchResult Result { get; }

        string Language { get; }
    }
}
