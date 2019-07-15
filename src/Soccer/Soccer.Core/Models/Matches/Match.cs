using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Models.Leagues;
using Soccer.Core.Models.Teams;

namespace Soccer.Core.Models.Matches
{
    internal class Match : Entity<string, string>
    {
        public DateTime EventDate { get; set; }

        public IEnumerable<Team> Teams { get; set; }

        public MatchResult MatchResult { get; set; }

        public League League { get; set; }

        public LeagueRound LeagueRound { get; set; }

        public IEnumerable<Timeline> TimeLines { get; set; }

        public int Attendance { get; set; }

        public Venue Venue { get; set; }

        public string Referee { get; set; }

        public string Region { get; set; }

        public Timeline LatestTimeline { get; set; }

        public IEnumerable<MatchFunction> Functions { get; set; }

        public Match ChangeEventDateByTimeZone(TimeSpan timeZone)
        {
            EventDate = EventDate.ToUniversalTime() + timeZone;

            return this;
        }
    }
}