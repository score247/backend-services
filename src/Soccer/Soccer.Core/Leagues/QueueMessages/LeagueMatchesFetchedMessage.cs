using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeagueMatchesFetchedMessage
    {
        IEnumerable<LeagueSeasonProcessedInfo> LeagueSeasons { get; }
    }
    public class LeagueMatchesFetchedMessage : ILeagueMatchesFetchedMessage
    {
        public LeagueMatchesFetchedMessage(IEnumerable<LeagueSeasonProcessedInfo> leagueSeasons)
        {
            LeagueSeasons = leagueSeasons;
        }

        public IEnumerable<LeagueSeasonProcessedInfo> LeagueSeasons { get; }
    }
}
