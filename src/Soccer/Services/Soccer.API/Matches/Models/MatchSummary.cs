namespace Soccer.API.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public class MatchSummary
    {
        public MatchSummary(Match match)
        {
            Id = match.Id;
            EventDate = match.EventDate;
            LeagueId = match.League.Id;
            LeagueName = match.League.Name;
            EventStatus = match.MatchResult.EventStatus;
            MatchStatus = match.MatchResult.MatchStatus;

            var teams = match.Teams.ToList();
            var homeTeam = teams.FirstOrDefault(t => t.IsHome);
            var awayTeam = teams.FirstOrDefault(t => !t.IsHome);

            HomeTeamId = homeTeam.Id;
            HomeTeamName = homeTeam.Name;
            AwayTeamId = awayTeam.Name;
            AwayTeamName = awayTeam.Name;
            HomeScore = (byte)match.MatchResult.HomeScore;
            AwayScore = (byte)match.MatchResult.AwayScore;
            AggregateWinnerId = match.MatchResult.AggregateWinnerId;
            HomeRedCards = (byte)homeTeam.Statistic.RedCards;
            HomeYellowRedCards = (byte)homeTeam.Statistic.YellowRedCards;
            AwayRedCards = (byte)awayTeam.Statistic.RedCards;
            AwayYellowRedCards = (byte)awayTeam.Statistic.YellowRedCards;
            MatchTime = (byte)match.MatchResult.MatchTime;
            StoppageTime = match.LatestTimeline?.StoppageTime;
            InjuryTimeAnnounced = (byte)(match.LatestTimeline?.InjuryTimeAnnounced ?? 0);
            LastTimelineType = match.LatestTimeline?.Type;
            MatchPeriods = match.MatchResult.MatchPeriods;
        }

        public string Id { get; }

        public DateTimeOffset EventDate { get; }

        public string LeagueId { get; }

        public string LeagueName { get; }

        public string HomeTeamId { get; }

        public string HomeTeamName { get; }

        public string AwayTeamId { get; }

        public string AwayTeamName { get; }

        public MatchStatus EventStatus { get; }

        public MatchStatus MatchStatus { get; }

        public byte HomeScore { get; }

        public byte AwayScore { get; }

        public string AggregateWinnerId { get; }

        public byte HomeRedCards { get; }

        public byte HomeYellowRedCards { get; }

        public byte AwayRedCards { get; }

        public byte AwayYellowRedCards { get; }

        public byte MatchTime { get; }

        public string StoppageTime { get; }

        public byte InjuryTimeAnnounced { get; }

        public EventType LastTimelineType { get; }

        public IEnumerable<MatchPeriod> MatchPeriods { get; private set; }
    }
}