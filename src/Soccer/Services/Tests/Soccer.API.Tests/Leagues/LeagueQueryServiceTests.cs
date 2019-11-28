using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.API.Leagues;
using Soccer.API.Shared.Configurations;
using Soccer.Cache.Leagues;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Database.Leagues.Criteria;
using Xunit;

namespace Soccer.API.Tests.Leagues
{
    public class LeagueQueryServiceTests
    {
        private readonly IAppSettings appSetting;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueCache leagueCache;
        private readonly LeagueQueryService leagueQueryService;

        public LeagueQueryServiceTests()
        {
            appSetting = Substitute.For<IAppSettings>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            leagueCache = Substitute.For<ILeagueCache>();

            leagueQueryService = new LeagueQueryService(appSetting, dynamicRepository, leagueCache);
        }

        [Fact]
        public async Task GetLeagueTable_NoData_ReturnDefaultLeagueTable()
        {
            var leagueTable = await leagueQueryService.GetLeagueTable("id", "seasonid", "groupname", Language.en_US);

            Assert.Null(leagueTable.League);
        }

        [Fact]
        public async Task GetLeagueTable_NoGroupNameSpecific_ReturnAllGroupTables()
        {
            var leagueId = "id";
            var seasonId = "seasonid";
            var groupName = string.Empty;
            StubLeagueTable(leagueId, seasonId, Language.en_US);

            var leagueTable = await leagueQueryService.GetLeagueTable(leagueId, seasonId, groupName, Language.en_US);

            Assert.Equal(2, leagueTable.GroupTables.Count());
            Assert.Equal("group1", leagueTable.GroupTables.First().Id);
            Assert.Equal("group2", leagueTable.GroupTables.ElementAt(1).Id);
        }

        [Fact]
        public async Task GetLeagueTable_GroupNameSpecific_ReturnSpecifiedGroup()
        {
            var leagueId = "id1";
            var seasonId = "seasonid1";
            var groupName = "tablename-group1";
            StubLeagueTable(leagueId, seasonId, Language.en_US);

            var leagueTable = await leagueQueryService.GetLeagueTable(leagueId, seasonId, groupName, Language.en_US);

            Assert.Single(leagueTable.GroupTables);
            Assert.Equal("group1", leagueTable.GroupTables.First().Id);
        }

        [Fact]
        public async Task GetLeagueTable_Alway_ReturnGroupedOutcome()
        {
            var leagueId = "id1";
            var seasonId = "seasonid1";
            var groupName = "tablename-group1";
            StubLeagueTable(leagueId, seasonId, Language.en_US);

            var leagueTable = await leagueQueryService.GetLeagueTable(leagueId, seasonId, groupName, Language.en_US);

            var firstTable = leagueTable.GroupTables.First();
            Assert.Equal(3, firstTable.OutcomeList.Count());
            Assert.Equal(TeamOutcome.AFCChampionsLeague, firstTable.OutcomeList.First());
            Assert.Equal(TeamOutcome.Promotion, firstTable.OutcomeList.ElementAt(1));
            Assert.Equal(TeamOutcome.Relegation, firstTable.OutcomeList.ElementAt(2));
        }

        private void StubLeagueTable(string leagueId, string seasonId, Language language)
        {
            var leagueTable = new LeagueTable(
                new League(leagueId, "name"),
                LeagueTableType.TotalTable,
                new LeagueSeason(seasonId, "seasonName", DateTimeOffset.Now, DateTimeOffset.Now, "1989", "leagueId"),
                new List<LeagueGroupTable>
                {
                    StubLeagueGroupTable("group1"),
                    StubLeagueGroupTable("group2")
                });

            dynamicRepository
                .GetAsync<LeagueTable>(Arg.Is<GetLeagueTableCriteria>(criteria
                    => criteria.LeagueId == leagueId
                        && criteria.SeasonId == seasonId
                        && criteria.Language == language.DisplayName))
                .Returns(leagueTable);
        }

        private LeagueGroupTable StubLeagueGroupTable(string id)
            => new LeagueGroupTable(
                id,
                $"tablename-{id}",
                new List<LeagueGroupNote>
                {
                    new LeagueGroupNote($"team1-{id}", "team1Name", new List<string> { "Comment1" })
                },
                new List<TeamStanding>
                {
                     A.Dummy<TeamStanding>().With(t => t.Name, $"team1-{id}").With(t => t.Outcome, TeamOutcome.AFCChampionsLeague),
                     A.Dummy<TeamStanding>().With(t => t.Name, $"team2-{id}").With(t => t.Outcome, TeamOutcome.Promotion),
                     A.Dummy<TeamStanding>().With(t => t.Name, $"team3-{id}").With(t => t.Outcome, TeamOutcome.Relegation)
                });
    }
}