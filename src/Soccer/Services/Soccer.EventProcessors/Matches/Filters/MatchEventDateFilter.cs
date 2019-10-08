using System;
using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Matches.Filters
{
    public class MatchEventDateFilter : IFilter<IEnumerable<Match>, IEnumerable<Match>>
    {
        private const int TimeSpanInMinutes = 10;

        public IEnumerable<Match> Filter(IEnumerable<Match> data)
            => data.Where(m => m.MatchResult.EventStatus != MatchStatus.NotStarted 
                                || (m.MatchResult.EventStatus == MatchStatus.NotStarted && IsValidEventDate(m.EventDate)));

        private static bool IsValidEventDate(DateTimeOffset dateTime)
            => dateTime >= (DateTimeOffset.Now - TimeSpan.FromMinutes(TimeSpanInMinutes));       
    }
}
