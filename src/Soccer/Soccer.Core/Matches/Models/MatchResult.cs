using System.Collections.Generic;
using Newtonsoft.Json;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.Models
{
    public class MatchResult
    {
#pragma warning disable S107 // Methods should not have too many parameters

        [JsonConstructor]
        public MatchResult(
            MatchStatus eventStatus,
            MatchStatus matchStatus,
            int period,
            IEnumerable<MatchPeriod> matchPeriods,
            int matchTime,
            string winnerId,
            int homeScore,
            int awayScore,
            int aggregateHomeScore,
            int aggregateAwayScore,
            string aggregateWinnerId)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            EventStatus = eventStatus;
            MatchStatus = matchStatus;
            Period = period;
            MatchPeriods = matchPeriods;
            MatchTime = matchTime;
            WinnerId = winnerId;
            HomeScore = homeScore;
            AwayScore = awayScore;
            AggregateHomeScore = aggregateHomeScore;
            AggregateAwayScore = aggregateAwayScore;
            AggregateWinnerId = aggregateWinnerId;
        }

        // TODO: Move out
        public MatchStatus EventStatus { get; }

        public MatchStatus MatchStatus { get; }

        public int Period { get; }

        public IEnumerable<MatchPeriod> MatchPeriods { get; }

        public int MatchTime { get; }

        public string WinnerId { get; }

        public int HomeScore { get; }

        public int AwayScore { get; }

        public int AggregateHomeScore { get; }

        public int AggregateAwayScore { get; }

        public string AggregateWinnerId { get; }
    }
}