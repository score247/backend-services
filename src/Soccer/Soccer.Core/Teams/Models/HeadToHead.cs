using Newtonsoft.Json;
using Score247.Shared.Base;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Teams.Models
{
    public class HeadToHead : BaseEntity
    {
        [JsonConstructor]
        public HeadToHead(string homeTeamId, string awayTeamId, Match match) : base(string.Empty)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            Match = match;
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public Match Match { get; }
    }
}