namespace Soccer.Core.Domain.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using Soccer.Core.Base;
    using Soccer.Core.Domain.Leagues.Models;
    using Soccer.Core.Domain.Teams.Models;

    public class Match : BaseEntity
    {
        public DateTime EventDate { get; set; }

        public IEnumerable<Team> Teams { get; set; }

        public MatchResult MatchResult { get; set; }

        public League League { get; set; }

        public LeagueRound LeagueRound { get; set; }

        public IEnumerable<Timeline> TimeLines { get; set; }

        public Timeline LatestTimeline { get; set; }

        public int Attendance { get; set; }

        public Venue Venue { get; set; }

        public string Referee { get; set; }

        public string Region { get; set; }

        public Match ChangeEventDateByTimeZone(TimeSpan timeZone)
        {
            EventDate = EventDate.ToUniversalTime() + timeZone;

            return this;
        }
    }
}