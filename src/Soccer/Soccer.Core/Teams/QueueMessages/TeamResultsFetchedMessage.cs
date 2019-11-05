using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Teams.QueueMessages
{
    public interface ITeamResultsFetchedMessage
    {
        Match TeamResult { get; }

        Language Language { get; }
    }

    public class TeamResultsFetchedMessage : ITeamResultsFetchedMessage
    {
        public TeamResultsFetchedMessage(Match teamResult, Language language)
        {
            TeamResult = teamResult;
            Language = language;
        }

        public Match TeamResult { get; }

        public Language Language { get; }
    }
}