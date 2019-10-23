namespace Soccer.Core.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using Score247.Shared.Base;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Teams.Models;
    using Soccer.Core.Timeline.Models;

    public sealed class Match : BaseEntity
    {
#pragma warning disable S107 // Methods should not have too many parameters

        public Match(
            string id,
            DateTimeOffset eventDate,
            DateTimeOffset currentPeriodStartTime,
            IEnumerable<Team> teams,
            MatchResult matchResult,
            League league,
            LeagueRound leagueRound,
            IEnumerable<TimelineEvent> timeLines,
            TimelineEvent latestTimeline,
            int attendance,
            Venue venue,
            string referee,
            string region,
            Coverage coverage,
            IEnumerable<TimelineCommentary> timelineCommentaries) : base(id)
        {
            EventDate = eventDate;
            CurrentPeriodStartTime = currentPeriodStartTime;
            Teams = teams;
            MatchResult = matchResult;
            League = league;
            LeagueRound = leagueRound;
            TimeLines = timeLines;
            LatestTimeline = latestTimeline;
            Attendance = attendance;
            Venue = venue;
            Referee = referee;
            Region = region;
            Coverage = coverage;
            TimelineCommentaries = timelineCommentaries;
        }

#pragma warning restore S107 // Methods should not have too many parameters

        public DateTimeOffset EventDate { get; }

        public DateTimeOffset CurrentPeriodStartTime { get; set; }

        public IEnumerable<Team> Teams { get; }

        public MatchResult MatchResult { get; }

        public League League { get; private set; }

        public LeagueRound LeagueRound { get; }

        public IEnumerable<TimelineEvent> TimeLines { get; set; }

        public TimelineEvent LatestTimeline { get; set; }

        public int Attendance { get; }

        public Venue Venue { get; }

        public string Referee { get; }

        public string Region { get; }

        public Coverage Coverage { get; }

        public IEnumerable<TimelineCommentary> TimelineCommentaries { get; }

        public void SetLeague(League league) => League = league;
    }
}