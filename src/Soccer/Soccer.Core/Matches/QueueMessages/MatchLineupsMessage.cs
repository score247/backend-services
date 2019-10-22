using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface IMatchLineupsMessage
    {
        Match MatchLineups { get; }

        Language Language { get; }
    }

    public class MatchLineupsMessage : IMatchLineupsMessage
    {
        public MatchLineupsMessage(Match matchLineups, Language language)
        {
            MatchLineups = matchLineups;
            Language = language;
        }

        public Match MatchLineups { get; }

        public Language Language { get; }
    }
}