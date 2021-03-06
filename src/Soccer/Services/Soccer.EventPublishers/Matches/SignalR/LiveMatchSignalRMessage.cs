namespace Soccer.EventPublishers.Matches.SignalR
{
    using System.Collections.Generic;
    using System.Linq;
    using Soccer.Core.Matches.Models;

    public class LiveMatchSignalRMessage
    {
        public LiveMatchSignalRMessage(
            byte sportId,
            IEnumerable<MatchSummary> newMatches,
            IEnumerable<MatchSummary> removedMatches)
        {
            SportId = sportId;
            NewMatches = newMatches;
            RemoveMatchIds = removedMatches.Select(m => m.Id).ToArray();
        }

        public byte SportId { get; }

        public IEnumerable<MatchSummary> NewMatches { get; }

        public string[] RemoveMatchIds { get; }
    }
}