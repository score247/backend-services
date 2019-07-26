namespace Soccer.Core.Matches.Events
{
    using System.Collections.Generic;
    using Soccer.Core.Matches.Models;

    public interface IPreMatchesFetchedEvent
    {
        IReadOnlyList<Match> Matches { get; }

        string Language { get; }
    }
}