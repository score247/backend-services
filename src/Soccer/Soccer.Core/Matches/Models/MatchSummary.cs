using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
#pragma warning disable S109 // Magic numbers should not be used
    public class MatchSummary
    {
#pragma warning disable S107 // Methods should not have too many parameters

        [SerializationConstructor]
        public MatchSummary(
            string id,
            DateTimeOffset eventDate,
            DateTimeOffset currentPeriodStartTime,
            string leagueId,
            string leagueName,
            string homeTeamId,
            string homeTeamName,
            string awayTeamId,
            string awayTeamName,
            MatchStatus matchStatus,
            MatchStatus eventStatus,
            byte homeScore,
            byte awayScore,
            string winnerId,
            string aggregateWinnerId,
            byte aggregateHomeScore,
            byte aggregateAwayScore,
            byte homeRedCards,
            byte homeYellowRedCards,
            byte awayRedCards,
            byte awayYellowRedCards,
            byte matchTime,
            string stoppageTime,
            byte injuryTimeAnnounced,
            EventType lastTimelineType,
            IEnumerable<MatchPeriod> matchPeriods,
            string countryCode,
            string countryName,
            DateTimeOffset modifiedTime,
            bool isInternationalLeague,
            int leagueOrder,
            string leagueSeasonId,
            LeagueRoundType leagueRoundType,
            string leagueRoundName,
            int leagueRoundNumber,
            string leagueRoundGroup,
            string leagueAbbreviation,
            byte homeYellowCards,
            byte awayYellowCards)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            Id = id;
            EventDate = eventDate;
            CurrentPeriodStartTime = currentPeriodStartTime;
            LeagueId = leagueId;
            LeagueName = leagueName;
            HomeTeamId = homeTeamId;
            HomeTeamName = homeTeamName;
            AwayTeamId = awayTeamId;
            AwayTeamName = awayTeamName;
            MatchStatus = matchStatus;
            EventStatus = eventStatus;
            HomeScore = homeScore;
            AwayScore = awayScore;
            WinnerId = winnerId;
            AggregateWinnerId = aggregateWinnerId;
            AggregateHomeScore = aggregateHomeScore;
            AggregateAwayScore = aggregateAwayScore;
            HomeRedCards = homeRedCards;
            HomeYellowRedCards = homeYellowRedCards;
            AwayRedCards = awayRedCards;
            AwayYellowRedCards = awayYellowRedCards;
            MatchTime = matchTime;
            StoppageTime = stoppageTime;
            InjuryTimeAnnounced = injuryTimeAnnounced;
            LastTimelineType = lastTimelineType;
            MatchPeriods = matchPeriods;
            CountryCode = countryCode;
            CountryName = countryName;
            ModifiedTime = modifiedTime;
            IsInternationalLeague = isInternationalLeague;
            LeagueOrder = leagueOrder;
            LeagueSeasonId = leagueSeasonId;
            LeagueRoundType = leagueRoundType;
            LeagueRoundName = leagueRoundName;
            LeagueRoundNumber = leagueRoundNumber;
            LeagueRoundGroup = leagueRoundGroup;
            LeagueAbbreviation = leagueAbbreviation;
            HomeYellowCards = homeYellowCards;
            AwayYellowCards = awayYellowCards;
        }

        public MatchSummary()
        {
        }

        public MatchSummary(Match match)
        {
            Id = match.Id;
            EventDate = match.EventDate;
            CurrentPeriodStartTime = match.CurrentPeriodStartTime;
            ModifiedTime = match.ModifiedTime;

            AssignLeague(match);
            AssignMatchResult(match);
            AssignTeamInformation(match);
            AssignLatestTimeline(match);

            Coverage = match.Coverage;
        }

        private void AssignLeague(Match match)
        {
            if (match.League != null)
            {
                LeagueId = match.League.Id;
                LeagueName = match.League.Name;
                CountryCode = match.League.CountryCode;
                CountryName = match.League.CountryName;
                IsInternationalLeague = match.League.IsInternational;
                LeagueOrder = match.League.Order;
                LeagueSeasonId = match.LeagueSeason?.Id;
                LeagueRoundGroup = match.LeagueRound?.Group;
                LeagueRoundType = match.LeagueRound?.Type;
                LeagueRoundNumber = match.LeagueRound?.Number ?? 0;
                LeagueRoundName = match.LeagueRound?.Name;
                LeagueGroupName = string.IsNullOrWhiteSpace(match.LeagueGroupName)
                    ? match.League.MapLeagueGroupName(match.LeagueRound, Language.en_US)
                    : match.LeagueGroupName;
                LeagueAbbreviation = match.League.Abbreviation;
            }
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
                AggregateHomeScore = (byte)match.MatchResult.AggregateHomeScore;
                AggregateAwayScore = (byte)match.MatchResult.AggregateAwayScore;
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
                        HomeYellowCards = (byte)homeTeam.Statistic.YellowCards;

                    }
                }

                if (awayTeam != null)
                {
                    AwayTeamId = awayTeam.Id;
                    AwayTeamName = awayTeam.Name;

                    if (awayTeam.Statistic != null)
                    {
                        AwayRedCards = (byte)awayTeam.Statistic.RedCards;
                        AwayYellowRedCards = (byte)awayTeam.Statistic.YellowRedCards;
                        AwayYellowCards = (byte)awayTeam.Statistic.YellowCards;
                    }
                }
            }
        }

        public string Id { get; }

        public DateTimeOffset EventDate { get; }

        public DateTimeOffset CurrentPeriodStartTime { get; }

        public string LeagueId { get; private set; }

        public string LeagueName { get; private set; }

        public string HomeTeamId { get; private set; }

        public string HomeTeamName { get; private set; }

        public string AwayTeamId { get; private set; }

        public string AwayTeamName { get; private set; }

        public MatchStatus MatchStatus { get; private set; }

        public MatchStatus EventStatus { get; private set; }

        public byte HomeScore { get; private set; }

        public byte AwayScore { get; private set; }

        public string WinnerId { get; private set; }

        public string AggregateWinnerId { get; private set; }

        public byte AggregateHomeScore { get; private set; }

        public byte AggregateAwayScore { get; private set; }

        public byte HomeRedCards { get; private set; }

        public byte HomeYellowRedCards { get; private set; }
        public byte AwayRedCards { get; private set; }

        public byte AwayYellowRedCards { get; private set; }

        public byte MatchTime { get; private set; }

        public string StoppageTime { get; private set; }

        public byte InjuryTimeAnnounced { get; private set; }

        public EventType LastTimelineType { get; private set; }

        public IEnumerable<MatchPeriod> MatchPeriods { get; private set; }

        public string CountryCode { get; private set; }

        public string CountryName { get; private set; }

        public DateTimeOffset ModifiedTime { get; private set; }

        public bool IsInternationalLeague { get; private set; }

        public int LeagueOrder { get; private set; }

        public string LeagueSeasonId { get; private set; }

        public LeagueRoundType LeagueRoundType { get; private set; }

        public string LeagueRoundName { get; private set; }

        public int LeagueRoundNumber { get; private set; }

        public string LeagueRoundGroup { get; private set; }

        public string LeagueGroupName { get; private set; }

        public Coverage Coverage { get; private set; }

        public string LeagueAbbreviation { get; private set; }

        public byte HomeYellowCards { get; private set; }

        public byte AwayYellowCards { get; private set; }

        /// <summary>
        /// For testing javscript messagepack deserialization
        /// </summary>
        public DateTime EventDateServerTime => EventDate.DateTime;
    }

#pragma warning restore S109 // Magic numbers should not be used
}