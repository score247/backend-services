using System.Collections.Generic;
using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    public static class LineupsMapper
    {
        private const string Home = "home";

        public static MatchLineups MapLineups(MatchLineupsDto matchLineupsDto, string region)
        {
            var match = MatchMapper.MapMatch(matchLineupsDto.sport_event, region);

            var homeTeam = MapTeamLineups(match.Teams.FirstOrDefault(t => t.IsHome), matchLineupsDto.lineups);
            var awayTeam = MapTeamLineups(match.Teams.FirstOrDefault(t => !t.IsHome), matchLineupsDto.lineups);

            return new MatchLineups(match.Id, match.EventDate, homeTeam, awayTeam);
        }

        private static TeamLineups MapTeamLineups(Team team, IEnumerable<Lineup> lineups)
        {
            if (team != null)
            {
                var teamLineups = lineups.FirstOrDefault(t => (team.IsHome && t.team == Home) || (!team.IsHome && t.team != Home));

                if (teamLineups != null)
                {
                    var startingLineups = teamLineups.starting_lineup.Select(pl =>
                            new Player(
                                pl.id,
                                pl.name,
                                Enumeration.FromDisplayName<PlayerType>(pl.type),
                                pl.jersey_number,
                                Enumeration.FromDisplayName<Position>(pl.position),
                                pl.order));

                    var substitutesLineups = teamLineups.substitutes.Select(pl => new Player(
                            pl.id,
                            pl.name,
                            Enumeration.FromDisplayName<PlayerType>(pl.type),
                            pl.jersey_number,
                            Position.Unknown,
                            0));
                    var coach = new Coach(
                            teamLineups.manager?.id,
                            teamLineups.manager?.country_code,
                            teamLineups.manager?.nationality,
                            teamLineups.manager?.name
                        );

                    return new TeamLineups(
                        team.Id,
                        team.Name,
                        team.IsHome,
                        coach,
                        teamLineups.formation,
                        startingLineups,
                        substitutesLineups);
                }
            }

            return default(TeamLineups);
        }
    }
}