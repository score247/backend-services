using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
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
        public string TeamId { get; }

        public string TeamName { get; }

#pragma warning restore S109 // Magic numbers should not be used
        public IEnumerable<string> Comments { get; }
    }
}