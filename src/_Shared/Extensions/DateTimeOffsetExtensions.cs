using System;

namespace Score247.Shared.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        private const int DateSpan = 3;

        public static bool IsFormer(this DateTimeOffset dt, DateTimeOffset currentDate)
            => dt.ToUniversalTime().Date < currentDate.ToUniversalTime().AddDays(-DateSpan).Date;

        public static bool IsAhead(this DateTimeOffset dt, DateTimeOffset currentDate)
            => dt.ToUniversalTime().Date > currentDate.ToUniversalTime().AddDays(DateSpan).Date;

        public static bool IsCurrent(this DateTimeOffset dt, DateTimeOffset currentDate)
            => dt.ToUniversalTime() <= currentDate.ToUniversalTime().AddDays(DateSpan).Date &&
             dt.ToUniversalTime() >= currentDate.ToUniversalTime().AddDays(-DateSpan).Date;
    }
}
