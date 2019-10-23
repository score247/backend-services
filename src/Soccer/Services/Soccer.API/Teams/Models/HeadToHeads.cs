using System.Collections.Generic;
using MessagePack;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Teams.Models
{
    [MessagePackObject]
    public class HeadToHeads
    {
        public HeadToHeads(IEnumerable<Team> teams, IEnumerable<MatchSummary> matches)
        {
            Teams = teams;
            Matches = matches;
        }

        [Key(0)]
        public IEnumerable<Team> Teams { get; }

        [Key(1)]
        public IEnumerable<MatchSummary> Matches { get; }
    }
}