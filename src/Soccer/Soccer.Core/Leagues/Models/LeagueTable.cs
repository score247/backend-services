using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
using Soccer.Core._Shared.Enumerations;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class LeagueTable
    {
        [JsonConstructor]
        public LeagueTable(
            League league,
            LeagueTableType type,
            LeagueSeason leagueSeason,
            IEnumerable<LeagueGroupTable> groupTables)
        {
            League = league;
            Type = type;
            LeagueSeason = leagueSeason;
            GroupTables = groupTables;
        }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(0)]
        public League League { get; }

        [Key(1)]
        public LeagueTableType Type { get; }

        [Key(2)]
        public LeagueSeason LeagueSeason { get; }

        [Key(3)]
        public IEnumerable<LeagueGroupTable> GroupTables { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}