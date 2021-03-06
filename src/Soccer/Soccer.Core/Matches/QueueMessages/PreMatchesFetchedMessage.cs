using System.Collections.Generic;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface IPreMatchesFetchedMessage
    {
        IEnumerable<Match> Matches { get; }

        string Language { get; }
    }

    public class PreMatchesFetchedMessage : IPreMatchesFetchedMessage
    {
        public PreMatchesFetchedMessage(IEnumerable<Match> matches, string language)
        {
            Matches = matches;
            Language = language;
        }

        public IEnumerable<Match> Matches { get; private set; }

        public string Language { get; private set; }
    }
}