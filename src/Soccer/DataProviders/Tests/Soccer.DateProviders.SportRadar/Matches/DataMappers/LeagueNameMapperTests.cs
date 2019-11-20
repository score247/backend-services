using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Xunit;

namespace Soccer.DateProviders.SportRadar.Matches.DataMappers
{
    [Trait("Soccer.DateProviders.SportRadar", "LeagueMapper")]
    public class LeagueNameMapperTests
    {
        [Theory]
        [InlineData("categoryName", "countryCode", "tournamentName", "B", "categoryName tournamentName:: Group B")]
        [InlineData("categoryName", "countryCode", "tournamentName", "1B", "categoryName tournamentName:: Group B")]
        [InlineData("categoryName", "", "tournamentName", "1B", "tournamentName:: Group B")]
        [InlineData("categoryName", "countryCode", "tournamentName", "tournamentName", "categoryName tournamentName")]
        public void MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(
            string categoryName,  string countryCode, string tournamentName, string group, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(group: group));

            var leagueGroupName = LeagueMapper.MapLeagueGroupName(league, leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryName", "countryCode", "tournamentName", "roundName", "categoryName tournamentName:: roundName")]
        [InlineData("categoryName", "countryCode", "tournamentName", "round_Name", "categoryName tournamentName:: round Name")]
        [InlineData("categoryName", "", "tournamentName", "1B", "tournamentName:: 1B")]
        public void MapLeagueName_Rule1_TypeIsCup_ReturnLeagueGroupName(
           string categoryName, string countryCode, string tournamentName, string name, string expectedLeagueName)
        {
            var league = LeagueMapper.MapLeague(StubTournamentDto(categoryName, countryCode, tournamentName), "region");
            var leagueRound = LeagueMapper.MapLeagueRound(StubTournamentRoundDto(type:"cup", name: name));

            var leagueGroupName = LeagueMapper.MapLeagueGroupName(league, leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament, subname", "B", "categoryName tournament:: subname")]
        [InlineData("categoryName", "", "tournament, subname", "tournament, subname", "tournament:: subname")]
        public void MapLeagueName_Rule2_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
            => MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(categoryName, countryCode, tournamentName, group, expectedLeagueName);

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament, subname1, subname2", "B", "categoryName tournament:: subname1:: subname2")]
        [InlineData("categoryName", "", "tournament, subname1, subname2", "tournament, subname", "tournament:: subname1:: subname2")]
        public void MapLeagueName_Rule3_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
            => MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(categoryName, countryCode, tournamentName, group, expectedLeagueName);

        [Theory]
        [InlineData("categoryName", "countryCode", "tournament", "A", "", "playoffs", "categoryName tournament:: Group A:: Playoffs")]
        [InlineData("categoryName", "", "tournament", "A", "roundname", "playoffs", "tournament:: Group A:: Playoffs")]
        [InlineData("categoryName", "countryCode", "tournament", "", "roundname", "playoffs", "categoryName tournament:: roundname:: Playoffs")]
        [InlineData("categoryName", "", "tournament", "", "roundname", "playoffs", "tournament:: roundname:: Playoffs")]
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

            var leagueGroupName = LeagueMapper.MapLeagueGroupName(league, leagueRound, Language.en_US);

            Assert.Equal(expectedLeagueName, leagueGroupName);
        }

        [Theory]
        [InlineData("categoryNameMain, CategorySub", "countryCode", "tournamentName", "B", "categoryNameMain tournamentName:: Group B")]
        [InlineData("categoryNameMain, CategorySub", "", "tournamentName", "1B", "tournamentName:: Group B")]
        public void MapLeagueName_Rule5_ReturnLeagueGroupName(
            string categoryName, string countryCode, string tournamentName, string group, string expectedLeagueName)
                => MapLeagueName_Rule1_TypeIsGroup_ReturnLeagueGroupName(categoryName, countryCode, tournamentName, group, expectedLeagueName);


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
