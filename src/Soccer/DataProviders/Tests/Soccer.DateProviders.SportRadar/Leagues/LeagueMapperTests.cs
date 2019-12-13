using System;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Xunit;

namespace Soccer.DateProviders.SportRadar.Leagues
{
    public class LeagueMapperTests
    {
        [Fact]
        public void MapLeagueSeason_NullSeasonDto_Null()
        {
            // Arrange

            SeasonDto seasonDto = null;

            // Act

            var leagueSeason = LeagueMapper.MapLeagueSeason(seasonDto);

            // Arrange

            Assert.Null(leagueSeason);
        }

        [Fact]
        public void MapLeagueSeason_NotNullSeasonDto_CorrectLeagueSeason()
        {
            // Arrange

            SeasonDto seasonDto = new SeasonDto
            {
                id = "sr:season:888",
                name = "Valid Season",
                end_date = "2020-10-30T00:00:00+00:00",
                start_date = "2019-01-01T00:00:00+00:00",
                tournament_id = "sr:tournament:999",
                year = "19-20"
            };

            var endDate = new DateTimeOffset(2020, 10, 30, 0, 0, 0, TimeSpan.Zero);
            var startDate = new DateTimeOffset(2019, 01, 01, 0, 0, 0, TimeSpan.Zero);

            // Act

            var leagueSeason = LeagueMapper.MapLeagueSeason(seasonDto);

            // Arrange

            Assert.Equal("sr:season:888", leagueSeason.Id);
            Assert.Equal("Valid Season", leagueSeason.Name);
            Assert.Equal("sr:tournament:999", leagueSeason.LeagueId);
            Assert.Equal("19-20", leagueSeason.Year);
            Assert.Equal(startDate, leagueSeason.StartDate);
            Assert.Equal(endDate, leagueSeason.EndDate);
        }
    }
}