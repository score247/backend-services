using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Matches.Filters
{
    public class MatchEventDateFilter : IFilter<IEnumerable<Match>, IEnumerable<Match>>
    {
        private const int TimeSpanInMinutes = 10;

        public Task<IEnumerable<Match>> FilterAsync(IEnumerable<Match> data)
            => Task.FromResult(data.Where(m => m.MatchResult.EventStatus != MatchStatus.NotStarted 
                                                || (m.MatchResult.EventStatus == MatchStatus.NotStarted && IsValid(m.EventDate))));

        private static bool IsValid(DateTimeOffset dateTime)
            => dateTime >= (DateTimeOffset.Now - TimeSpan.FromMinutes(TimeSpanInMinutes));       
    }
}
