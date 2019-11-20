using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Teams.QueueMessages
{
    public interface IHeadToHeadFetchedMessage
    {
        Match Match { get; }

        Language Language { get; }
    }

    public class HeadToHeadFetchedMessage : IHeadToHeadFetchedMessage
    {
        public HeadToHeadFetchedMessage(Match teamResult, Language language)
        {
            Match = teamResult;
            Language = language;
        }

        public Match Match { get; }

        public Language Language { get; }
    }
}