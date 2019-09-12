namespace Soccer.API.Matches.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using MessagePack;
    using Soccer.Core.Matches.Models;

    [MessagePackObject]
    public class MatchInfo
    {
        public MatchInfo(
            MatchSummary match,
            IEnumerable<TimelineEvent> timelineEvents,
            Venue venue,
            string referee,
            int attendance)
        {
            Match = match;
            TimelineEvents = timelineEvents.OrderBy(t => t.Time).ToList();
            Attendance = attendance;
            Venue = venue;
            Referee = referee;
        }

        [Key(0)]
        public MatchSummary Match { get; }

        [Key(1)]
        public IEnumerable<TimelineEvent> TimelineEvents { get; }

        [Key(2)]
        public Venue Venue { get; }

        [Key(3)]
        public string Referee { get; }

        [Key(4)]
        public int Attendance { get; }
    }
}