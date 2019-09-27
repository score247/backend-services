using System.Collections.Generic;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface ILiveMatchUpdatedMessage
    {
        Language Language { get; }

        IEnumerable<Match> NewMatches { get; }

        IEnumerable<Match> RemovedMatches { get; }
    }

    public class LiveMatchUpdatedMessage : ILiveMatchUpdatedMessage
    {
        public LiveMatchUpdatedMessage(Language language, IEnumerable<Match> newMatches, IEnumerable<Match> removedMatches)
        {
            Language = language;
            NewMatches = newMatches;
            RemovedMatches = removedMatches;
        }

        public Language Language { get; }

        public IEnumerable<Match> NewMatches { get; }

        public IEnumerable<Match> RemovedMatches { get; }
    }
}
