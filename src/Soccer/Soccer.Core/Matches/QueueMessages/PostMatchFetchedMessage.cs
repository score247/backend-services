using System.Collections.Generic;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface IPostMatchFetchedMessage
    {
        IEnumerable<Match> Matches { get; }

        string Language { get; }
    }

    public class PostMatchFetchedMessage : IPostMatchFetchedMessage
    {
        public PostMatchFetchedMessage(IEnumerable<Match> matches, string language)
        {
            Matches = matches;
            Language = language;
        }

        public IEnumerable<Match> Matches { get; private set; }

        public string Language { get; private set; }
    }
}