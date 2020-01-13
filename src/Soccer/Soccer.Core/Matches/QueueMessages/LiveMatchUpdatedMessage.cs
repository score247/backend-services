using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface ILiveMatchUpdatedMessage
    {
        Language Language { get; }

        IEnumerable<MatchSummary> NewMatches { get; }

        IEnumerable<MatchSummary> RemovedMatches { get; }
    }

    public class LiveMatchUpdatedMessage : ILiveMatchUpdatedMessage
    {
        public LiveMatchUpdatedMessage(Language language, IEnumerable<Match> newMatches, IEnumerable<Match> removedMatches)
        {
            Language = language;
            NewMatches = newMatches.Select(m => new MatchSummary(m));
            RemovedMatches = removedMatches.Select(m => new MatchSummary(m));
        }

        public Language Language { get; }

        public IEnumerable<MatchSummary> NewMatches { get; }

        public IEnumerable<MatchSummary> RemovedMatches { get; }
    }
}