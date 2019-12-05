namespace Soccer.API.Matches.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using MessagePack;
    using Soccer.Core.Matches.Models;

    [MessagePackObject(keyAsPropertyName: true)]
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
            TimelineEvents = timelineEvents.OrderBy(t => t.MatchTime).ThenBy(t => t.Time).ToList();
            Attendance = attendance;
            Venue = venue;
            Referee = referee;
        }

#pragma warning disable S109 // Magic numbers should not be used

        public MatchSummary Match { get; }

        public IEnumerable<TimelineEvent> TimelineEvents { get; }

        public Venue Venue { get; }

        public string Referee { get; }

        public int Attendance { get; }
#pragma warning restore S109 // Magic numbers should not be used
    }
}