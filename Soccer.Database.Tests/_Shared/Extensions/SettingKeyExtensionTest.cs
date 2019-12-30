using System;
using AutoFixture;
using Soccer.Database._Shared.Extensions;
using Xunit;

namespace Soccer.Database.Tests._Shared.Extensions
{
    public class SettingKeyExtensionTest
    {
        private readonly Fixture fixture;

        public SettingKeyExtensionTest()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void GetCorrespondingKey_FormerAtBound_GetFormer(int date)
        {
            var sp = fixture.Create<string>();
            var currentDate = new DateTimeOffset(new DateTime(2019, 12, 11, 7, 0, 0), TimeSpan.FromHours(7));
            var eventDate = new DateTimeOffset(new DateTime(2019, 12, date, 7, 0, 0), TimeSpan.FromHours(7));

            var settingKey = sp.GetCorrespondingKey(eventDate, currentDate);

            Assert.Equal($"{sp}_Former", settingKey);
        }

        [Fact]
        public void GetCorrespondingKey_DefaultOfDateTime_GetCurrent()
        {
            var sp = fixture.Create<string>();

            var settingKey = sp.GetCorrespondingKey(default, DateTimeOffset.Now);

            Assert.Equal(sp, settingKey);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        public void GetCorrespondingKey_InCurrentRange_GetCurrent(int date)
        {
            var sp = fixture.Create<string>();
            var currentDate = new DateTimeOffset(new DateTime(2019, 12, 11, 7, 0, 0), TimeSpan.FromHours(7));
            var eventDate = new DateTimeOffset(new DateTime(2019, 12, date, 7, 0, 0), TimeSpan.FromHours(7));

            var settingKey = sp.GetCorrespondingKey(eventDate, currentDate);

            Assert.Equal(sp, settingKey);
        }

        [Theory]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        public void GetCorrespondingKey_GetAhead(int date)
        {
            var sp = fixture.Create<string>();
            var currentDate = new DateTimeOffset(new DateTime(2019, 12, 11, 7, 0, 0), TimeSpan.FromHours(7));
            var eventDate = new DateTimeOffset(new DateTime(2019, 12, date, 7, 0, 0), TimeSpan.FromHours(7));

            var settingKey = sp.GetCorrespondingKey(eventDate, currentDate);

            Assert.Equal($"{sp}_Ahead", settingKey);
        }
    }
}
