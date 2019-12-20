using System;
using System.Collections.Generic;
using Score247.Shared.Extensions;
using Soccer.API.Matches.Models;

namespace Soccer.API.Matches.Helpers
{
    public static class DateRangeHelper
    {
        private const int DateSpan = 3;

        public static IList<DateRange> GenerateDateRanges(DateTimeOffset from, DateTimeOffset to)
        {
            var currentDate = DateTimeOffset.UtcNow;

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
                    currentTo = EndOfCurrentRange();

                    var aheadFrom = from.IsAhead(currentDate) ? from : BeginningOfAheadRange();

                    dateRanges.Add(new DateRange(aheadFrom, to));
                }

                if (currentTo > currentFrom)
                {
                    dateRanges.Add(new DateRange(currentFrom, currentTo, true));
                }
            }

            return dateRanges;
        }

        private static DateTimeOffset BeginningOfAheadRange()
            => new DateTimeOffset(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(DateSpan + 1), TimeSpan.Zero);

        private static DateTimeOffset EndOfCurrentRange()
            => BeginningOfAheadRange().AddTicks(-1);
    }
}
