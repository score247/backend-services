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

        public void FormatPlayerEventStatistic()
        {
            FormatEventStatistic(Players);
            FormatEventStatistic(Substitutions);
        }

        private static void FormatEventStatistic(IEnumerable<Player> players)
        {
            if (players != null)
            {
                foreach (var player in players)
                {
                    player.FormatEventStatistic();
                }
            }
        }
    }
}