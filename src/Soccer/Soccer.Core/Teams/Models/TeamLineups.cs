using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TeamLineups : Team
    {
        public static readonly IEnumerable<EventType> LineupsEvents = new List<EventType>
        {
            EventType.ScoreChange,
            EventType.RedCard,
            EventType.YellowCard,
            EventType.YellowRedCard,
            EventType.Substitution
        };

        private const char formationSplitChar = '-';

        [SerializationConstructor, JsonConstructor]
        public TeamLineups(
            string id,
            string name,
            bool isHome,
            Coach coach,
            string formation,
            IEnumerable<Player> players,
            IEnumerable<Player> substitutions)
            : base(id, name, string.Empty, string.Empty, string.Empty, isHome, default(TeamStatistic), string.Empty)
        {
            Coach = coach;
            Formation = formation;
            Players = players;
            Substitutions = substitutions;
        }

        public Coach Coach { get; }

        public string Formation { get; }

        public IEnumerable<Player> Players { get; }

        public IEnumerable<Player> Substitutions { get; }

        public IEnumerable<TimelineEvent> SubstitutionEvents { get; set; }

        public IList<byte> ConvertFormationToList()
        {
            var tempList = (Formation ?? string.Empty)
                           .Split(formationSplitChar)
                           .Where(fm => !string.IsNullOrWhiteSpace(fm))
                           .Select(byte.Parse)
                           .ToList();
            var formationList = new List<byte> { 1 };
            formationList.AddRange(tempList);

            return formationList;
        }
    }

    public class TeamLineups2 : TeamLineups
    {
        [SerializationConstructor, JsonConstructor]
        public TeamLineups2(
            string id,
            string name,
            bool isHome,
            Coach coach,
            string formation,
            IEnumerable<Player> players,
            IEnumerable<Player> substitutions)
            : base(id, name, isHome, coach, formation, players.Select(player => new PlayerLineups(player)), substitutions.Select(player => new PlayerLineups(player)))
        {
        }

        public TeamLineups2(TeamLineups teamLineups)
            : this(teamLineups.Id, teamLineups.Name, teamLineups.IsHome, teamLineups.Coach, teamLineups.Formation, teamLineups.Players, teamLineups.Substitutions)
        {
            SubstitutionEvents = teamLineups.SubstitutionEvents;
        }
    }
}