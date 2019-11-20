using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeaguesSeasonFetchedMessage
    {
        IEnumerable<League> Leagues { get; }
    }

    public class LeaguesSeasonFetchedMessage : ILeaguesSeasonFetchedMessage
    {
        public LeaguesSeasonFetchedMessage(IEnumerable<League> leagues)
        {
            Leagues = leagues;
        }

        public IEnumerable<League> Leagues { get; }
    }
}