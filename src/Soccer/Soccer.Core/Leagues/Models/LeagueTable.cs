using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;
using Soccer.Core._Shared.Enumerations;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class LeagueTable
    {
        public LeagueTable() { }

        [JsonConstructor, SerializationConstructor]
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
        public IEnumerable<LeagueGroupTable> GroupTables { get; private set; }


        public void FilterAndCalculateGroupTableOutcome(string groupName)
        {
            if (GroupTables == null || !GroupTables.Any())
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(groupName) && GroupTables.Count() > 1)
            {
                groupName = groupName.Trim();
                GroupTables = GroupTables.Where(table => groupName.Equals(table?.Name, StringComparison.InvariantCultureIgnoreCase));
            }

            foreach (var groupTable in GroupTables)
            {
                groupTable.CalculateOutcomeList();
            }
        }

#pragma warning restore S109 // Magic numbers should not be used
    }
}