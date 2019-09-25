namespace Soccer.Core.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using Score247.Shared.Base;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Teams.Models;

    public sealed class Match : BaseEntity
    {
        public DateTimeOffset EventDate { get; set; }

        public DateTimeOffset CurrentPeriodStartTime { get; set; }

        public IEnumerable<Team> Teams { get; set; }

        public MatchResult MatchResult { get; set; }

        public League League { get; set; }

        public LeagueRound LeagueRound { get; set; }

        public IEnumerable<TimelineEvent> TimeLines { get; set; }

        public TimelineEvent LatestTimeline { get; set; }

        public int Attendance { get; set; }

        public Venue Venue { get; set; }

        public string Referee { get; set; }

        public string Region { get; set; }

        public Coverage Coverage { get; set; }
    }
}