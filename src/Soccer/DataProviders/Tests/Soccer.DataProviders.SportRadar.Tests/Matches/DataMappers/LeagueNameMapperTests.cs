using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Xunit;

namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    [Trait("Soccer.DataProviders.SportRadar", "LeagueMapper")]
    public class LeagueNameMapperTests
    {
        [Theory]
        [InlineData("categoryName", "countryCode", "tournamentName", "B", "categoryName tournamentName:: Group B")]
        [InlineData("categoryName", "countryCode", "tournamentName", "1B", "categoryName tournamentName:: Group B")]
        [InlineData("categoryName", "", "tournamentName", "1B", "tournamentName:: Group B")]
        [InlineData("categoryName", "countryCode", "tournamentName", "tournamentName", "categoryName tournamentName")]
        [InlineData("categoryName", "countryCode", "tournamentName", "TOurnamentName", "categoryName tournamentName")]
        [InlineData("categoryName", "countryCode", "tournamentNAME", "TOurnamentName", "categoryName tournamentNAME")]
        public void MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(group: group));

            var leagueGroupName = league.MapLeagueGroupName(leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryName", "countryCode", "tournamentName", "roundName", "categoryName tournamentName:: phase:: roundName")]
        [InlineData("categoryName", "countryCode", "tournamentName", "round_Name", "categoryName tournamentName:: phase:: round Name")]
        [InlineData("categoryName", "", "tournamentName", "1B", "tournamentName:: phase:: 1B")]        
        public void MapLeagueName_Rule1_TypeIsCup_ReturnLeagueGroupName(
           string categoryName, string countryCode, string tournamentName, string name, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(type: "cup", name: name));

            var leagueGroupName = league.MapLeagueGroupName(leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament, subname", "B", "categoryName tournament subname:: Group B")]
        [InlineData("categoryName", "countryCode", "tournament, subname", "subname", "categoryName tournament subname")]
        [InlineData("categoryName", "countryCode", "tournament, subname", "Subname", "categoryName tournament subname")]
        [InlineData("categoryName", "", "tournament, subname", "tournament, subname", "tournament subname")]
        public void MapLeagueName_Rule2_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
            => MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(categoryName, countryCode, tournamentName, group, expectedLeagueName);

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament, subname1, subname2", "B", "categoryName tournament subname2 subname1:: Group B")]
        [InlineData("categoryName", "countryCode", "tournament, subname1, subname2", "subname1", "categoryName tournament subname2 subname1")]
        [InlineData("categoryName", "countryCode", "tournament, subname1, subname2", "subname2", "categoryName tournament subname2 subname1")]
        [InlineData("categoryName", "", "tournament, subname1, subname2", "", "tournament subname2 subname1")]
        public void MapLeagueName_Rule3_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
            => MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(categoryName, countryCode, tournamentName, group, expectedLeagueName);

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament, subname1, subname2", "A", "", "playoffs", "categoryName tournament subname2 subname1:: Group A")]
        [InlineData("categoryName", "countryCode", "tournament, subname1", "A", "", "playoffs", "categoryName tournament subname1:: Group A")]
        [InlineData("categoryName", "countryCode", "tournament", "A", "", "playoffs", "categoryName tournament:: Group A")]
        [InlineData("categoryName", "", "tournament", "A", "roundname", "playoffs", "tournament:: Group A")]
        [InlineData("categoryName", "countryCode", "tournament", "", "roundname", "playoffs", "categoryName tournament:: roundname")]
        [InlineData("categoryName", "", "tournament", "", "roundname", "playoffs", "tournament:: roundname")]
        [InlineData("categoryName", "", "tournament, subname1, subname2", "A", "roundname", "playoffs", "tournament subname2 subname1:: Group A")]
        [InlineData("categoryName", "countryCode", "tournament, subname", "A", "", "playoffs", "categoryName tournament subname:: Group A")]
        [InlineData("categoryName", "", "tournament, subname", "A", "roundname", "playoffs", "tournament subname:: Group A")]
        [InlineData("categoryName", "countryCode", "tournament, subname1, subname2", "", "quaterfinal", "knockout_stage", "categoryName tournament subname2 subname1:: knockout stage:: quaterfinal")]
        public void MapLeagueName_Rule4_ReturnLeagueGroupName(
           string categoryName, string countryCode, string tournamentName, string group, string roundName, string phase, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var groupType = string.IsNullOrWhiteSpace(group) ? LeagueRoundType.CupRound : LeagueRoundType.GroupRound;
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(
                groupType.DisplayName,
                roundName,
                group,
                phase));

            var leagueGroupName = league.MapLeagueGroupName(leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryNameMain, CategorySub", "countryCode", "tournamentName", "B", "categoryNameMain tournamentName:: Group B")]
        [InlineData("categoryNameMain, CategorySub", "", "tournamentName", "1B", "tournamentName:: Group B")]
        public void MapLeagueName_Rule5_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
                => MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(categoryName, countryCode, tournamentName, group, expectedLeagueName);

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament", "variable", "categoryName tournament")]
        [InlineData("categoryName", "", "tournament", "variable", "tournament")]
        [InlineData("categoryName", "countryCode", "tournament", "unknown", "categoryName tournament")]
        [InlineData("categoryName", "", "tournament", "unknown", "tournament")]
        [InlineData("categoryName", "countryCode", "tournament", "playoff", "categoryName tournament")]
        [InlineData("categoryName", "", "tournament", "playoff", "tournament")]
        public void MapLeagueName_NotSupportRule_ReturnLeagueGroupName(
           string categoryName, string countryCode, string tournamentName, string typeName, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(typeName));

            var leagueGroupName = league.MapLeagueGroupName(leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament", "phase_1", "categoryName tournament:: phase 1:: tournament round name")]
        [InlineData("categoryName", "", "tournament", "phase_2", "tournament:: phase 2:: tournament round name")]
        public void MapLeagueName_Rule1_TypeIsQualification_ReturnLeagueGroupName(
           string categoryName, string countryCode, string tournamentName, string phase, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(type: LeagueRoundType.Qualifier, phase: phase));

            var leagueGroupName = league.MapLeagueGroupName(leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }


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

        private TournamentRoundDto StubTournamentRoundDto(
            string type = "group",
            string name = "tournament round name",
            string group = "B",
            string phase = "phase")
            => new TournamentRoundDto
            {
                type = type,
                name = name,
                group = group,
                phase = phase
            };
    }
}
