namespace Score247.Shared.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime ConvertToUtc(this DateTime dt, TimeSpan offset)
            => dt - offset;

        public static DateTime ConvertFromUtcToTimeZone(this DateTime dt, TimeSpan offset)
            => dt.ToUniversalTime() + offset;

        public static DateTime ConvertFromLocalToTimeZone(this DateTime dt, TimeSpan offset)
           => dt + (DateTimeOffset.Now.Offset - offset);

        public static DateTime EndDay(this DateTime dt)
           => dt.AddDays(1).AddSeconds(-1);
    }
}