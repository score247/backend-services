using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;
using Soccer.Core._Shared.Enumerations;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueTable
    {
        public LeagueTable()
        {
        }

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
        public League League { get; }

        public LeagueTableType Type { get; }

        public LeagueSeason LeagueSeason { get; }

        public IEnumerable<LeagueGroupTable> GroupTables { get; private set; }

        public void FilterAndCalculateGroupTableOutcome(string groupName)
        {
            if (GroupTables?.Any() != true)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(groupName) && GroupTables.Count() > 1)
            {
                groupName = groupName.Trim();
                GroupTables = GroupTables.Where(table => groupName.Equals(table?.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            foreach (var groupTable in GroupTables)
            {
                groupTable.CalculateOutcomeList();
            }
        }

#pragma warning restore S109 // Magic numbers should not be used
    }
}