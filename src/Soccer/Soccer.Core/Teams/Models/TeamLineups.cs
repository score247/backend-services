using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TeamLineups : Team
    {
        private const char formationSplitChar = '-';

#pragma warning disable S107 // Methods should not have too many parameters
        [SerializationConstructor, JsonConstructor]
        public TeamLineups(
            string id,
            string name,
            bool isHome,
            Coach coach,
            string formation,
            IEnumerable<Player> players,
            IEnumerable<Player> substitutions) : base(id, name, isHome)
#pragma warning restore S107 // Methods should not have too many parameters
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

        public IEnumerable<byte> FormationToArray() => (Formation ?? string.Empty)
            .Split(formationSplitChar)
            .Where(fm => !string.IsNullOrWhiteSpace(fm))
            .Select(fm => byte.Parse(fm));

        public byte TotalFormationLine() => (byte)(1 + FormationToArray().Count());
    }
}