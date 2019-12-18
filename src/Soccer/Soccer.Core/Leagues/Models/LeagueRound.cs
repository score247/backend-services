using Newtonsoft.Json;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueRound
    {
        [JsonConstructor]
        public LeagueRound(LeagueRoundType type, string name, int number, string phase, string group)
        {
            Type = type;
            Name = name;
            Number = number;
            Phase = phase;
            Group = group;
        }

        public LeagueRoundType Type { get; private set; }

        public string Name { get; }

        public int Number { get; }

        public string Phase { get; }

        public string Group { get; private set; }
    }
}