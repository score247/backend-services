using System;

namespace Soccer.API.Matches.Models
{
    public class DateRange
    {
        public DateRange(DateTimeOffset from, DateTimeOffset to, bool isCached = false)
        {
            From = from;
            To = to;
            IsCached = isCached;
        }

        public DateTimeOffset From { get; }

        public DateTimeOffset To { get; }

        public bool IsCached { get; }
    }
}
