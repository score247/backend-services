using System;
using System.Collections.Generic;
using System.Linq;
using Soccer.DataProviders.SportRadar.Matches.Dtos;
using Xunit;

namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    [Trait("Soccer.DataProviders.SportRadar", "MatchLineups")]
    public class LineupsMapperTests
    {
        [Fact]
        public void MapTeamLineups_DtoIsNull_ReturnDefaultTeamLineups()
        {
            var lineups = LineupsMapper.MapLineups(null, "region");

            Assert.Null(lineups);
        }

        [Fact]
        public void MapTeamLineups_SportEventHasData_ReturnMatchInfo()
        {
            var matchLineupsDto = StubMatchLineupsDto();

            var lineups = LineupsMapper.MapLineups(matchLineupsDto, "region");

            Assert.Equal("matchId", lineups.Id);
            Assert.Equal(new DateTime(1989, 5, 28), lineups.EventDate);
            Assert.Equal("homeid", lineups.Home.Id);
            Assert.Equal("team home", lineups.Home.Name);
            Assert.Equal("awayid", lineups.Away.Id);
            Assert.Equal("team away", lineups.Away.Name);
        }

        [Fact]
        public void MapTeamLineups_SportEventHasData_ReturnHomeLineups()
        {
            var matchLineupsDto = StubMatchLineupsDto();

            var homeLineups = LineupsMapper.MapLineups(matchLineupsDto, "region").Home;

            Assert.Equal("homeid", homeLineups.Id);
            Assert.Equal("team home", homeLineups.Name);
            Assert.Equal("4-3-4", homeLineups.Formation);
            Assert.Single(homeLineups.Players);
            Assert.Equal("starting home", homeLineups.Players.FirstOrDefault().Name);
            Assert.Equal("subtitute home", homeLineups.Substitutions.FirstOrDefault().Name);
            Assert.Equal("Manager home", homeLineups.Coach.Name);
        }

        [Fact]
        public void MapTeamLineups_SportEventHasData_ReturnAwayLineups()
        {
            var matchLineupsDto = StubMatchLineupsDto();

            var awayLineups = LineupsMapper.MapLineups(matchLineupsDto, "region").Away;

            Assert.Equal("awayid", awayLineups.Id);
            Assert.Equal("team away", awayLineups.Name);
            Assert.Equal("3-3-3-1", awayLineups.Formation);
            Assert.Single(awayLineups.Players);
            Assert.Equal("starting away", awayLineups.Players.FirstOrDefault().Name);
            Assert.Equal("subtitute away", awayLineups.Substitutions.FirstOrDefault().Name);
            Assert.Equal("Manager away", awayLineups.Coach.Name);
        }

        private MatchLineupsDto StubMatchLineupsDto(
            string id = "matchId")
            => new MatchLineupsDto
            {
                sport_event = new SportEventDto
                {
                    id = id,
                    scheduled = new DateTime(1989, 5, 28),
                    competitors = new List<CompetitorDto>
                    {
                        new CompetitorDto { id = "homeid", name = "team home", qualifier = "home" },
                        new CompetitorDto { id = "awayid", name = "team away", qualifier = "away" }
                    }
                },
                lineups = new List<Lineup>
                {
                    StubTeamLineup(),
                    StubTeamLineup("away", "3-3-3-1")
                }
            };

        private static Lineup StubTeamLineup(string team = "home", string formation = "4-3-4")
            => new Lineup
            {
                formation = formation,
                team = team,
                starting_lineup = new List<StartingLineup>
                {
                    new StartingLineup
                    {
                        name = $"starting {team}",
                        type = "goalkeeper",
                        position = "Goalkeeper"
                    }
                },
                substitutes = new List<Substitute>
                {
                    new Substitute
                    {
                        name = $"subtitute {team}",
                        type = "goalkeeper"
                    }
                },
                manager = new Manager
                {
                    id = "11",
                    name = $"Manager {team}"
                }
            };
    }
}