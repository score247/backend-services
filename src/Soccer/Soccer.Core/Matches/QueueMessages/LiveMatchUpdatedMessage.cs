namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;
    using System.Collections.Generic;

    public interface ILiveMatchResultUpdatedMessage
    {
        IEnumerable<Match> Matches { get; }
    }

    public class LiveMatchResultUpdatedMessage : ILiveMatchResultUpdatedMessage
    {
        public LiveMatchResultUpdatedMessage(IEnumerable<Match> matches)
        {            
            Matches = matches;
        }

        public IEnumerable<Match> Matches { get; }
    }
}