using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject]
    public class MatchCommentary
    {
        public MatchCommentary(string matchId, IEnumerable<TimelineEvent> timelines) 
        {
            MatchId = matchId;
            TimelineEvents = timelines.OrderBy(t => t.Time).ToList();
        }

        [Key(0)]
        public string MatchId { get; }

        [Key(1)]
        public IEnumerable<TimelineEvent> TimelineEvents { get; }
    }
}
