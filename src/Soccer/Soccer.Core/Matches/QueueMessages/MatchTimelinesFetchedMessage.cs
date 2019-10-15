using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface IMatchTimelinesFetchedMessage
    {
        Language Language { get; }

        Match Match { get; }
    }

    public class MatchTimelinesFetchedMessage : IMatchTimelinesFetchedMessage
    {
        public MatchTimelinesFetchedMessage(Match match, Language language)
        {
            Match = match;
            Language = language;
        }

        public Language Language { get; }

        public Match Match { get; }
    }
}
