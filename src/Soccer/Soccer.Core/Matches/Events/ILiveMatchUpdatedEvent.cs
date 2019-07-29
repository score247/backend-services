namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;
    using System.Collections.Generic;

    public interface ILiveMatchUpdatedEvent
    {
        IReadOnlyList<Match> Matches { get; }

        string Language { get; }
    }
}
