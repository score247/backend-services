using Soccer.Database.Leagues.Criteria;
using Xunit;

namespace Soccer.Database.Tests.Leagues.Criteria
{
    public class GetUnprocessedLeagueSeasonCriteriaTests
    {
        private readonly GetUnprocessedLeagueSeasonCriteria criteria;

        public GetUnprocessedLeagueSeasonCriteriaTests()
        {
            criteria = new GetUnprocessedLeagueSeasonCriteria();
        }

        [Fact]
        public void GetSettingKey_Always_ReturnCorrectSpName()
        {
            Assert.Equal("League_GetUnprocessedLeagueSeason", criteria.GetSettingKey());
        }

        [Fact]
        public void IsValid_Always_True()
        {
            Assert.True(criteria.IsValid());
        }
    }
}
