namespace Soccer.Core.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Score247.Shared.Base;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Teams.Models;

    public sealed class Match : BaseEntity
    {
#pragma warning disable S107 // Methods should not have too many parameters

        [JsonConstructor]
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
            LeagueSeason leagueSeason,
            string leagueGroupName,
            string groupName,
            LeagueGroupStage leagueGroupStage,
            InjuryTimes injuryTimes) : base(id)
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
            LeagueSeason = leagueSeason;
            LeagueGroupName = leagueGroupName;
            GroupName = groupName;
            LeagueGroupStage = leagueGroupStage;
            InjuryTimes = injuryTimes ?? new InjuryTimes();
        }

#pragma warning restore S107 // Methods should not have too many parameters

        public DateTimeOffset EventDate { get; private set; }

        public DateTimeOffset CurrentPeriodStartTime { get; set; }

        public IEnumerable<Team> Teams { get; private set; }

        public MatchResult MatchResult { get; private set; }

        public League League { get; private set; }

        public LeagueRound LeagueRound { get; private set; }

        public IEnumerable<TimelineEvent> TimeLines { get; private set; }

        public TimelineEvent LatestTimeline { get; set; }

        public int Attendance { get; private set; }

        public Venue Venue { get; private set; }

        public string Referee { get; private set; }

        public string Region { get; private set; }

        public Coverage Coverage { get; private set; }

        public LeagueSeason LeagueSeason { get; private set; }

        public string LeagueGroupName { get; private set; }

        public LeagueGroupStage LeagueGroupStage { get; private set; }

        public string GroupName { get; private set; }

        public InjuryTimes InjuryTimes { get; private set; }

        public void SetTimelines(IEnumerable<TimelineEvent> timelineEvents) => TimeLines = timelineEvents;

        public void UpdateLeagueGroupStage(LeagueGroupStage leagueGroupStage) => LeagueGroupStage = leagueGroupStage;
    }
}