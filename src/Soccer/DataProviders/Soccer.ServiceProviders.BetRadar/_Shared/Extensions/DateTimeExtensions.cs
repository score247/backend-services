namespace Soccer.DataProviders.SportRadar.Shared.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToSportRadarFormat(this DateTime dt)
             => dt.ToString("yyyy-MM-dd");
    }
}