﻿using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Core.Leagues.QueueMessages
{
    public interface ILeaguesFetchedMessage
    {
        IEnumerable<League> Leagues { get; }

        string Language { get; }
    }

    public class LeaguesSyncedMessage : ILeaguesFetchedMessage
    {
        public LeaguesSyncedMessage(IEnumerable<League> leagues, string language)
        {
            Leagues = leagues;
            Language = language;
        }

        public IEnumerable<League> Leagues { get; }

        public string Language { get; }
    }
}