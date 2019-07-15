namespace Soccer.Core.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime ConvertToUtc(this DateTime dt, TimeSpan timeZone)
        {
            return dt - timeZone;
        }
    }
}