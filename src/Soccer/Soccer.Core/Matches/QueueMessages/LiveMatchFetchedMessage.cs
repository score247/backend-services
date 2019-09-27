namespace Soccer.Core.Matches.Events
{
    using System.Collections.Generic;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public interface ILiveMatchFetchedMessage
    {
        Language Language { get; }

        IEnumerable<Match> Matches { get; }
    }

    public class LiveMatchFetchedMessage : ILiveMatchFetchedMessage
    {
        public LiveMatchFetchedMessage(Language language, IEnumerable<Match> matches)
        {
            Language = language;
            Matches = matches;
        }

        public Language Language { get; }

        public IEnumerable<Match> Matches { get; }
    }
}