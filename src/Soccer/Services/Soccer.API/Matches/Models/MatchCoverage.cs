using MessagePack;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject]
    public class MatchCoverage
    {
        public MatchCoverage(string matchId, Coverage coverage)
        {
            MatchId = matchId;
            Coverage = coverage;
        }

        [Key(0)]
        public string MatchId { get; }

        [Key(1)]
        public Coverage Coverage { get; }
    }
}
