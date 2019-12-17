using System;
using System.Linq;
using Soccer.API.Matches.Helpers;
using Xunit;

namespace Soccer.API.Tests.Matches.Helpers
{
    public class DateRangeHelperTests
    {
        [Fact]
        public void GenerateDateRanges_InFormerRange_CorrectFromToAndNotCached() 
        {
            var from = new DateTimeOffset(DateTime.UtcNow.AddDays(-5), TimeSpan.Zero);
            var to = new DateTimeOffset(DateTime.UtcNow.AddDays(-4), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Single(dateRanges);
            Assert.False(dateRanges.First().IsCached);

            Assert.Equal(from.Date, dateRanges.First().From.Date);
            Assert.Equal(from.Hour, dateRanges.First().From.Hour);
            Assert.Equal(from.Minute, dateRanges.First().From.Minute);

            Assert.Equal(to.Date, dateRanges.First().To.Date);
            Assert.Equal(to.Hour, dateRanges.First().To.Hour);
            Assert.Equal(to.Minute, dateRanges.First().To.Minute);
        }

        [Fact]
        public void GenerateDateRanges_InFormerBound_CorrectFromToAndNotCached()
        {
            var currentDate = DateTime.UtcNow;
            var from = new DateTimeOffset(currentDate.AddDays(-4), TimeSpan.Zero);
            var to = new DateTimeOffset(currentDate.AddDays(-3), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Single(dateRanges);
            Assert.False(dateRanges.First().IsCached);

            Assert.Equal(from.Date, dateRanges.First().From.Date);
            Assert.Equal(from.Hour, dateRanges.First().From.Hour);
            Assert.Equal(from.Minute, dateRanges.First().From.Minute);

            Assert.Equal(to.Date, dateRanges.First().To.Date);
            Assert.Equal(to.Hour, dateRanges.First().To.Hour);
            Assert.Equal(to.Minute, dateRanges.First().To.Minute);
        }

        [Fact]
        public void GenerateDateRanges_InCurrentRange_CorrectFromToAndCached()
        {
            var from = new DateTimeOffset(DateTime.UtcNow.AddDays(-3), TimeSpan.Zero);
            var to = new DateTimeOffset(DateTime.UtcNow.AddDays(3), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Single(dateRanges);
            Assert.True(dateRanges.First().IsCached);

            Assert.Equal(from.Date, dateRanges.First().From.Date);
            Assert.Equal(from.Hour, dateRanges.First().From.Hour);
            Assert.Equal(from.Minute, dateRanges.First().From.Minute);

            Assert.Equal(to.Date, dateRanges.First().To.Date);
            Assert.Equal(to.Hour, dateRanges.First().To.Hour);
            Assert.Equal(to.Minute, dateRanges.First().To.Minute);
        }

        [Fact]
        public void GenerateDateRanges_InAheadRange_CorrectFromToAndNotCached()
        {
            var from = new DateTimeOffset(DateTime.UtcNow.AddDays(4), TimeSpan.Zero);
            var to = new DateTimeOffset(DateTime.UtcNow.AddDays(5), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Single(dateRanges);
            Assert.False(dateRanges.First().IsCached);

            Assert.Equal(from.Date, dateRanges.First().From.Date);
            Assert.Equal(from.Hour, dateRanges.First().From.Hour);
            Assert.Equal(from.Minute, dateRanges.First().From.Minute);

            Assert.Equal(to.Date, dateRanges.First().To.Date);
            Assert.Equal(to.Hour, dateRanges.First().To.Hour);
            Assert.Equal(to.Minute, dateRanges.First().To.Minute);
        }

        [Fact]
        public void GenerateDateRanges_InFormerAndCurrentRange_CorrectFromToAndCached()
        {
            var from = new DateTimeOffset(DateTime.UtcNow.AddDays(-4), TimeSpan.Zero);
            var to = new DateTimeOffset(DateTime.UtcNow.AddDays(-2), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Equal(2, dateRanges.Count);
            Assert.Equal(1, dateRanges.Count(dt => dt.IsCached));
            Assert.Equal(1, dateRanges.Count(dt => !dt.IsCached));
        }

        [Fact]
        public void GenerateDateRanges_InCurrentAndAheadRange_CorrectFromToAndCached()
        {
            var from = new DateTimeOffset(DateTime.UtcNow.AddDays(2), TimeSpan.Zero);
            var to = new DateTimeOffset(DateTime.UtcNow.AddDays(4), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Equal(2, dateRanges.Count);
            Assert.Equal(1, dateRanges.Count(dt => dt.IsCached));
            Assert.Equal(1, dateRanges.Count(dt => !dt.IsCached));
        }

        [Fact]
        public void GenerateDateRanges_FormerToCurrentToAhead_CorrectFromToAndCached()
        {
            var from = new DateTimeOffset(DateTime.UtcNow.AddDays(-4), TimeSpan.Zero);
            var to = new DateTimeOffset(DateTime.UtcNow.AddDays(4), TimeSpan.Zero);

            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);

            Assert.Equal(3, dateRanges.Count);
            Assert.Equal(1, dateRanges.Count(dt => dt.IsCached));
            Assert.Equal(2, dateRanges.Count(dt => !dt.IsCached));
        }
    }
}
