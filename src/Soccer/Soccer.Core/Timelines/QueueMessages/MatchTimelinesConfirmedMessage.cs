using System;
using System.Collections.Generic;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Timelines.QueueMessages
{
    public interface IMatchTimelinesConfirmedMessage
    {
        string MatchId { get; }

        DateTimeOffset EventDate { get; }

        MatchStatus EventStatus { get; }

        IList<TimelineEvent> Timelines { get; }
    }

    public class MatchTimelinesConfirmedMessage : IMatchTimelinesConfirmedMessage
    {
        public MatchTimelinesConfirmedMessage(
            string matchId,
            DateTimeOffset eventDate,
            MatchStatus eventStatus,
            IList<TimelineEvent> timelines)
        {
            MatchId = matchId;
            EventDate = eventDate;
            EventStatus = eventStatus;
            Timelines = timelines;
        }

        public string MatchId { get; private set; }

        public DateTimeOffset EventDate { get; private set; }

        public MatchStatus EventStatus { get; private set; }

        public IList<TimelineEvent> Timelines { get; private set; }
    }
}
