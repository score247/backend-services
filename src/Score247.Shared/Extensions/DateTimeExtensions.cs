namespace Score247.Shared.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime ConvertToUtc(this DateTime dt, TimeSpan timeZone)
            => dt - timeZone;

        public static DateTime ConvertFromUtcToTimeZone(this DateTime dt, TimeSpan timeZone)
            => dt.ToUniversalTime() + timeZone;

        public static DateTime ConvertFromLocalToTimeZone(this DateTime dt, TimeSpan timeZone)
           => dt + (DateTimeOffset.Now.Offset - timeZone);
    }
}