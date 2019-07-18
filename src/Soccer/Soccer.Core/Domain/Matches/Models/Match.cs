namespace Soccer.Core.Domain.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using Score247.Shared.Base;
    using Score247.Shared.Extensions;
    using Soccer.Core.Domain.Leagues.Models;
    using Soccer.Core.Domain.Teams.Models;

    public sealed class Match : BaseModel, IEquatable<Match>
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

        public Match ChangeEventDateByTimeZone(TimeSpan offset)
        {
            EventDate = EventDate.ConvertFromUtcToOffset(offset);

            return this;
        }

        public bool Equals(Match other) => Id == other.Id;
    }
}