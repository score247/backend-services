using System.Collections.Generic;
using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar._Shared;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    public static class LineupsMapper
    {
        private const string Home = "home";

        public static MatchLineups MapLineups(MatchLineupsDto matchLineupsDto, string region)
        {
            if(matchLineupsDto == null)
            {
                return default;
            }

            var match = MatchMapper.MapMatch(matchLineupsDto.sport_event, null, null, region);

            var homeTeam = MapTeamLineups(match.Teams.FirstOrDefault(t => t.IsHome), matchLineupsDto.lineups);
            var awayTeam = MapTeamLineups(match.Teams.FirstOrDefault(t => !t.IsHome), matchLineupsDto.lineups);

            return new MatchLineups(match.Id, match.EventDate, homeTeam, awayTeam);
        }

        private static TeamLineups MapTeamLineups(Team team, IEnumerable<Lineup> lineups)
        {
            if (team != null && lineups != null)
            {
                var teamLineups = lineups.FirstOrDefault(t => (team.IsHome && t.team == Home) || (!team.IsHome && t.team != Home));

                if (teamLineups != null)
                {
                    var startingLineups = MapStartingLineups(teamLineups);
                    var substitutesLineups = MapSubstitutePlayers(teamLineups);
                    var coach = MapCoach(teamLineups);

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

        private static Coach MapCoach(Lineup teamLineups)
            => new Coach(
                    teamLineups.manager?.id,
                    PlayerNameConverter.Convert(teamLineups.manager?.name),
                    teamLineups.manager?.nationality,
                    teamLineups.manager?.country_code);

        private static IEnumerable<Player> MapSubstitutePlayers(Lineup teamLineups)
            => (teamLineups.substitutes ?? Enumerable.Empty<Substitute>())
                .OrderBy(st => st.jersey_number)
                .Select(pl => new Player(
                    pl.id,
                    PlayerNameConverter.Convert(pl.name),
                    string.IsNullOrWhiteSpace(pl.type) 
                        ? PlayerType.Unknown 
                        : Enumeration.FromDisplayName<PlayerType>(pl.type),
                    pl.jersey_number,
                    Position.Unknown,
                    0));

        private static IEnumerable<Player> MapStartingLineups(Lineup teamLineups)
            => (teamLineups.starting_lineup ?? Enumerable.Empty<StartingLineup>())
                .OrderBy(st => st.order)
                .Select(pl =>
                    new Player(
                        pl.id,
                        PlayerNameConverter.Convert(pl.name),
                        string.IsNullOrWhiteSpace(pl.type) 
                            ? PlayerType.Unknown 
                            : Enumeration.FromDisplayName<PlayerType>(pl.type),
                        pl.jersey_number,
                        string.IsNullOrWhiteSpace(pl.position) 
                            ? Position.Unknown 
                            : Enumeration.FromDisplayName<Position>(pl.position),
                        pl.order));
    }
}