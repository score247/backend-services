using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Xunit;

namespace Soccer.API.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var status = "live";

            var test = Enumeration.FromDisplayName<MatchStatus>(status);

            Assert.Equal("live", test.DisplayName);
        }
    }
}