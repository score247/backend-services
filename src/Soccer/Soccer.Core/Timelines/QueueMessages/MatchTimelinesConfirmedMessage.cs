using System;
using System.Collections.Generic;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Timelines.QueueMessages
{
    public interface IMatchTimelinesConfirmedMessage
    {
        string MatchId { get; }

        DateTimeOffset EventDate { get; }

        IList<TimelineEvent> Timelines { get; }
    }

    public class MatchTimelinesConfirmedMessage : IMatchTimelinesConfirmedMessage
    {
        public MatchTimelinesConfirmedMessage(string matchId, DateTimeOffset eventDate, IList<TimelineEvent> timelines)
        {
            MatchId = matchId;
            EventDate = eventDate;
            Timelines = timelines;
        }

        public string MatchId { get; private set; }

        public DateTimeOffset EventDate { get; private set; }

        public IList<TimelineEvent> Timelines { get; private set; }
    }
}
