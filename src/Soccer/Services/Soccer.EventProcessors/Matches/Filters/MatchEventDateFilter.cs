﻿using System;
using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Matches.Filters
{
    public class MatchEventDateFilter : IFilter<IEnumerable<Match>, IEnumerable<Match>>
    {
        private static TimeSpan TimeSpanInMinutes = TimeSpan.FromMinutes(10);
        private static DateTimeOffset ValidFromTime = DateTimeOffset.Now - TimeSpanInMinutes;
        private static DateTimeOffset ValidToTime = DateTimeOffset.Now + TimeSpanInMinutes;

        public IEnumerable<Match> Filter(IEnumerable<Match> data)
            => data.Where(m => m.MatchResult.EventStatus == MatchStatus.Live
                                || IsValidNotStartMatch(m.MatchResult.EventStatus, m.EventDate)
                                || IsValidClosedMatch(m.MatchResult.EventStatus, m.LatestTimeline?.Time));

        private static bool IsValidNotStartMatch(MatchStatus eventStatus, DateTimeOffset dateTime)
            => eventStatus.IsNotStart() 
                && dateTime >= ValidFromTime 
                && dateTime <= ValidToTime;

        private static bool IsValidClosedMatch(MatchStatus eventStatus, DateTimeOffset? dateTime)
            => (dateTime != null)
                && eventStatus.IsClosed()
                && dateTime >= ValidFromTime;
    }
}
