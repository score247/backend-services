using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Matches.Models
{
    public class MatchInfo
    {
        public MatchInfo(Match match, IEnumerable<TimelineEvent> timelineEvents)
        {
            Match = new MatchSummary(match);
            TimelineEvents = timelineEvents.OrderBy(t => t.Time);
            Attendance = match.Attendance;
            Venue = match.Venue;
            Referee = match.Referee;
        }

        public MatchSummary Match { get; }

        public IEnumerable<TimelineEvent> TimelineEvents { get; }

        public int Attendance { get; }

        public Venue Venue { get; }

        public string Referee { get; }
    }
}