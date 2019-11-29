using System;
using FakeItEasy;
using Soccer.Database._Shared.Extensions;
using Xunit;

namespace Soccer.Database.Tests._Shared.Extensions
{
    public class SettingKeyExtensionTest
    {
        [Fact]
        public void GetCorrespondingKey_GetFormer() 
        {
            var sp = A.Dummy<string>();

            var settingKey = sp.GetCorrespondingKey(DateTime.Now.AddDays(-3), DateTimeOffset.Now);

            Assert.Equal($"{sp}_Former", settingKey);
        }

        [Fact]
        public void GetCorrespondingKey_DefaultOfDateTime_GetCurrent()
        {
            var sp = A.Dummy<string>();

            var settingKey = sp.GetCorrespondingKey(default, DateTimeOffset.Now);

            Assert.Equal(sp, settingKey);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetCorrespondingKey_GetCurrent(int dateSpan)
        {
            var sp = A.Dummy<string>();

            var settingKey = sp.GetCorrespondingKey(DateTimeOffset.Now.AddDays(dateSpan), DateTimeOffset.Now);

            Assert.Equal(sp, settingKey);
        }

        [Fact]
        public void GetCorrespondingKey_GetAhead()
        {
            var sp = A.Dummy<string>();

            var settingKey = sp.GetCorrespondingKey(DateTimeOffset.Now.AddDays(3), DateTimeOffset.Now);

            Assert.Equal($"{sp}_Ahead", settingKey);
        }
    }
}
