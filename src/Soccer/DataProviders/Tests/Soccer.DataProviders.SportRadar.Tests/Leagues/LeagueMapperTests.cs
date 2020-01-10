using System;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Xunit;

namespace Soccer.DataProviders.SportRadar.Leagues
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
                end_date = DateTime.Parse("2020-10-30T00:00:00+00:00"),
                start_date = DateTime.Parse("2019-01-01T00:00:00+00:00"),
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

        [Theory]
        [InlineData("Hong Kong, China", "countryCode", "Hong Kong")]
        [InlineData("Scotland, UK", "countryCode", "Scotland")]
        public void MapLeague_CountryHasComma_ShouldFormat(string countryName, string countryCode, string expectedCountryName) 
        {
            var tournamentDetailDto = StubTournamentDetailDto(StubTournamentDto(countryName, countryCode, "Premier League"));

            var league = LeagueMapper.MapLeague(tournamentDetailDto, "region", Language.en_US);

            Assert.Equal(expectedCountryName, league.CountryName);
        }

        [Theory]
        [InlineData("tournament, subname", "tournament subname")]
        [InlineData("tournament, subname1, subname2", "tournament subname2 subname1")]
        public void MapLeague_LeagueNameHasComma_ShouldFormat(string leagueName, string expectedLeagueName)
        {
            var tournamentDetailDto = StubTournamentDetailDto(StubTournamentDto("", "", leagueName));

            var league = LeagueMapper.MapLeague(tournamentDetailDto, "region", Language.en_US);

            Assert.Equal(expectedLeagueName, league.Name);
        }

        private TournamentDetailDto StubTournamentDetailDto(TournamentDto tournamentDto)
            => new TournamentDetailDto 
            { 
                tournament = tournamentDto
            };

        private TournamentDto StubTournamentDto(string categoryName, string countryCode, string tournamentName)
            => new TournamentDto
            {
                id = "TournamentDtoId",
                name = tournamentName,
                category = new Category
                {
                    id = "CategoryId",
                    name = categoryName,
                    country_code = countryCode
                }
            };
    }
}