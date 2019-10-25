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

        [SerializationConstructor, JsonConstructor]
        public TeamLineups(
            string id,
            string name,
            string country,
            string countryCode,
            string flag,
            bool isHome,
            TeamStatistic statistic,
            string abbreviation,
            Coach coach,
            string formation,
            IEnumerable<Player> players,
            IEnumerable<Player> substitutions) : base(id, name, country, countryCode, flag, isHome, statistic, abbreviation)
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