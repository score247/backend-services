using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Xunit;

namespace Soccer.DataProviders.SportRadar.Tests.Leagues
{
    public class LeagueTeamPostfixTests
    {
        [Theory]
        [InlineData("U20 Womens World Cup", "U20 Women")]
        [InlineData("U19 European Championship Women", "U19 Women")]
        [InlineData("U17 Euro Ch.ship W", "U17 Women")]
        [InlineData("2. Bundesliga Women", "Women")]
        [InlineData("First Division, Women", "Women")]
        [InlineData("World Cup Qualification Inter-Confederation Playoffs Women", "Women")]
        [InlineData("Int. Friendly Games W", "Women")]
        public void GetTeamPostfix_LeagueUnderAgeOrWomenOrBoth_ShouldFormat(string leagueName, string expectedPostfix)
        {
            var tournamentDetailDto = StubTournamentDetailDto(StubTournamentDto("", "", leagueName));

            var league = LeagueMapper.MapLeague(tournamentDetailDto, "region", Language.en_US);

            Assert.Equal(expectedPostfix, league.GetTeamNamePostfix());
        }

        [Theory]
        [InlineData("UEFA Youth League", "Youth")]
        [InlineData("UEFA Youth League Women", "Youth Women")]
        [InlineData("Int. Friendly Games youth W", "Youth Women")]
        public void GetTeamPostfix_LeagueForYouthOrWomenOrBoth_ShouldFormat(string leagueName, string expectedPostfix)
        {
            var tournamentDetailDto = StubTournamentDetailDto(StubTournamentDto("", "", leagueName));

            var league = LeagueMapper.MapLeague(tournamentDetailDto, "region", Language.en_US);

            Assert.Equal(expectedPostfix, league.GetTeamNamePostfix());
        }

        [Theory]
        [InlineData("European Championship")]
        [InlineData("FA Cup")]
        [InlineData("World Cup Qualification CONCACAF")]
        [InlineData("World Cup")]
        [InlineData("Premier League")]
        public void GetTeamPostfix_LeagueNotContainsUnderAgeOrWomen_ShouldEmpty(string leagueName)
        {
            var tournamentDetailDto = StubTournamentDetailDto(StubTournamentDto("", "", leagueName));

            var league = LeagueMapper.MapLeague(tournamentDetailDto, "region", Language.en_US);

            Assert.Empty(league.GetTeamNamePostfix());
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
