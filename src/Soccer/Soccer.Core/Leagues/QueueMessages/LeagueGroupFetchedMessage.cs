using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueGroupFetchedMessage
    {
        LeagueGroupStage LeagueGroupStage { get; }

        Language Language { get; }
    }

    public class LeagueGroupFetchedMessage : ILeagueGroupFetchedMessage
    {
        public LeagueGroupFetchedMessage(LeagueGroupStage leagueGroupStage, Language language)
        {
            LeagueGroupStage = leagueGroupStage;
            Language = language;
        }

        public LeagueGroupStage LeagueGroupStage { get; }

        public Language Language { get; }
    }
}