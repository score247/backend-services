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
        public MatchStatus EventStatus { get; private set; }

        public MatchStatus MatchStatus { get; private set; }

        public int Period { get; private set; }

        public IEnumerable<MatchPeriod> MatchPeriods { get; private set; }

        public int MatchTime { get; private set; }

        public string WinnerId { get; private set; }

        public int HomeScore { get; private set; }

        public int AwayScore { get; private set; }

        public int AggregateHomeScore { get; private set; }

        public int AggregateAwayScore { get; private set; }

        public string AggregateWinnerId { get; private set; }
    }
}