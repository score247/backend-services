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

            if (match.League != null)
            {
                LeagueId = match.League.Id;
                LeagueName = match.League.Name;
            }

            AssignMatchResult(match);
            AssignTeamInformation(match);
            AssignLatestTimeline(match);
        }

        private void AssignLatestTimeline(Match match)
        {
            if (match.LatestTimeline != null)
            {
                StoppageTime = match.LatestTimeline.StoppageTime;
                InjuryTimeAnnounced = (byte)match.LatestTimeline.InjuryTimeAnnounced;
                LastTimelineType = match.LatestTimeline.Type;
            }
        }

        private void AssignMatchResult(Match match)
        {
            if (match.MatchResult != null)
            {
                EventStatus = match.MatchResult.EventStatus;
                MatchStatus = match.MatchResult.MatchStatus;
                HomeScore = (byte)match.MatchResult.HomeScore;
                AwayScore = (byte)match.MatchResult.AwayScore;
                WinnerId = match.MatchResult.WinnerId;
                AggregateWinnerId = match.MatchResult.AggregateWinnerId;
                MatchTime = (byte)match.MatchResult.MatchTime;
                MatchPeriods = match.MatchResult.MatchPeriods;
            }
        }

        private void AssignTeamInformation(Match match)
        {
            const int twoTeams = 2;
            if (match.Teams != null && match.Teams.Count() >= twoTeams)
            {
                var homeTeam = match.Teams.FirstOrDefault(t => t.IsHome);
                var awayTeam = match.Teams.FirstOrDefault(t => !t.IsHome);

                if (homeTeam != null)
                {
                    HomeTeamId = homeTeam.Id;
                    HomeTeamName = homeTeam.Name;

                    if (homeTeam.Statistic != null)
                    {
                        HomeRedCards = (byte)homeTeam.Statistic.RedCards;
                        HomeYellowRedCards = (byte)homeTeam.Statistic.YellowRedCards;
                    }
                }

                if (awayTeam != null)
                {
                    AwayTeamId = awayTeam.Name;
                    AwayTeamName = awayTeam.Name;

                    if (awayTeam.Statistic != null)
                    {
                        AwayRedCards = (byte)awayTeam.Statistic.RedCards;
                        AwayYellowRedCards = (byte)awayTeam.Statistic.YellowRedCards;
                    }
                }
            }
        }

        public string Id { get; private set; }

        public DateTimeOffset EventDate { get; private set; }

        public string LeagueId { get; private set; }

        public string LeagueName { get; private set; }

        public string HomeTeamId { get; private set; }

        public string HomeTeamName { get; private set; }

        public string AwayTeamId { get; private set; }

        public string AwayTeamName { get; private set; }

        public MatchStatus EventStatus { get; private set; }

        public MatchStatus MatchStatus { get; private set; }

        public byte HomeScore { get; private set; }

        public byte AwayScore { get; private set; }

        public string WinnerId { get; private set; }

        public string AggregateWinnerId { get; private set; }

        public byte HomeRedCards { get; private set; }

        public byte HomeYellowRedCards { get; private set; }

        public byte AwayRedCards { get; private set; }

        public byte AwayYellowRedCards { get; private set; }

        public byte MatchTime { get; private set; }

        public string StoppageTime { get; private set; }

        public byte InjuryTimeAnnounced { get; private set; }

        public EventType LastTimelineType { get; private set; }

        public IEnumerable<MatchPeriod> MatchPeriods { get; private set; }
    }
}