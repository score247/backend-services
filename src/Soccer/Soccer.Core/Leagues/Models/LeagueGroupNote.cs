using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class LeagueGroupNote
    {
        [JsonConstructor]
        public LeagueGroupNote(
            string teamId,
            string teamName,
            IEnumerable<string> comments)
        {
            TeamId = teamId;
            TeamName = teamName;
            Comments = comments;
        }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(0)]
        public string TeamId { get; }

        [Key(1)]
        public string TeamName { get; }

        [Key(2)]
#pragma warning restore S109 // Magic numbers should not be used
        public IEnumerable<string> Comments { get; }
    }
}