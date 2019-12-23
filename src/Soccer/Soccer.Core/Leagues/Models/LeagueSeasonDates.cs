using System;
using MessagePack;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueSeasonDates
    {
        public LeagueSeasonDates(DateTimeOffset startDate, DateTimeOffset endDate, string year)
        {
            StartDate = startDate;
            EndDate = endDate;
            Year = year;
        }

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset EndDate { get; private set; }

        public string Year { get; private set; }
    }
}