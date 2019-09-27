namespace Soccer.EventPublishers.Matches.SignalR
{
    using System.Collections.Generic;
    using System.Linq;
    using Soccer.Core.Matches.Models;

    public class LiveMatchSignalRMessage
    {
        public LiveMatchSignalRMessage(IEnumerable<MatchSummary> newMatches, IEnumerable<MatchSummary> removedMatches)
        {
            NewMatches = newMatches;
            RemoveMatchIds = removedMatches.Select(m => m.Id).ToArray();
        }

        public IEnumerable<MatchSummary> NewMatches { get; }

        public string[] RemoveMatchIds { get; }
    }
}