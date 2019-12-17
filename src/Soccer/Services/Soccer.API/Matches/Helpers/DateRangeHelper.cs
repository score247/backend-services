using System;
using System.Collections.Generic;
using Score247.Shared.Extensions;
using Soccer.API.Matches.Models;

namespace Soccer.API.Matches.Helpers
{
    public static class DateRangeHelper
    {
        public static IList<DateRange> GenerateDateRanges(DateTimeOffset from, DateTimeOffset to)
        {
            var currentDate = DateTimeOffset.UtcNow;
            var DateSpan = 3;

            var currentFrom = from;
            var currentTo = to;

            var dateRanges = new List<DateRange>();

            if (from.IsCurrent(currentDate) && to.IsCurrent(currentDate))
            {
                dateRanges.Add(new DateRange(from, to, true));
            }
            else
            {
                if (from.IsFormer(currentDate))
                {
                    currentFrom = to.IsFormer(currentDate) ? to : currentDate.AddDays(-DateSpan);

                    dateRanges.Add(new DateRange(from, currentFrom));
                }

                if (to.IsAhead(currentDate))
                {
                    currentTo = from.IsAhead(currentDate) ? from : currentDate.AddDays(DateSpan);

                    dateRanges.Add(new DateRange(currentTo, to));
                }

                if (currentTo > currentFrom)
                {
                    dateRanges.Add(new DateRange(currentFrom, currentTo, true));
                }
            }

            return dateRanges;
        }
    }
}
