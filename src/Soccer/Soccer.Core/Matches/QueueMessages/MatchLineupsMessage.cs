using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface IMatchLineupsMessage
    {
        MatchLineups MatchLineups { get; }

        Language Language { get; }
    }

    public class MatchLineupsMessage : IMatchLineupsMessage
    {
        public MatchLineupsMessage(MatchLineups matchLineups, Language language)
        {
            MatchLineups = matchLineups;
            Language = language;
        }

        public MatchLineups MatchLineups { get; }

        public Language Language { get; }
    }
}