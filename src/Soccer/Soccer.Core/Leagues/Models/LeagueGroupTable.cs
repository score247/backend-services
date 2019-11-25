using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class LeagueGroupTable
    {
        [JsonConstructor]
        public LeagueGroupTable(
            string id,
            string name,
            IEnumerable<LeagueGroupNote> groupNotes,
            IEnumerable<TeamStanding> teamStandings)
        {
            Id = id;
            Name = name;
            GroupNotes = groupNotes;
            TeamStandings = teamStandings;
        }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(0)]
        public string Id { get; }

        [Key(1)]
        public string Name { get; }

        [Key(2)]
        public IEnumerable<LeagueGroupNote> GroupNotes { get; }

        [Key(3)]
        public IEnumerable<TeamStanding> TeamStandings { get; }

        [Key(4)]
        public IEnumerable<TeamOutcome> OutcomeList { get; private set; }

        public void CalculateOutcomeList()
        {
            if (TeamStandings != null && TeamStandings.Any())
            {
                OutcomeList = TeamStandings
                    .Where(team => team.Outcome != null && team.Outcome != TeamOutcome.Unknown)
                    .GroupBy(team => team.Outcome)
                    .Select(group => group.Key);
            }
        }

#pragma warning restore S109 // Magic numbers should not be used
    }
}