namespace Soccer.Core.Domain.Matches.Events
{
    using System.Collections.Generic;
    using Soccer.Core.Domain.Matches.Models;

    public interface IPreMatchesFetchedEvent
    {
        IReadOnlyList<Match> Matches { get; }

        string Language { get; }
    }
}