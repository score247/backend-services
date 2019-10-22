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

        public static Match MapLineups(MatchLineupsDto matchLineupsDto, string region)
        {
            var match = MatchMapper.MapMatch(matchLineupsDto.sport_event, region);

            SetTeamLineups(match.Teams.FirstOrDefault(t => t.IsHome), matchLineupsDto.lineups);
            SetTeamLineups(match.Teams.FirstOrDefault(t => !t.IsHome), matchLineupsDto.lineups);

            return match;
        }

        private static void SetTeamLineups(Team team, IEnumerable<Lineup> lineups)
        {
            if (team != null)
            {
                var teamLineups = lineups.FirstOrDefault(t => (team.IsHome && t.team == Home) || (!team.IsHome && t.team != Home));

                if (teamLineups != null)
                {
                    team.Formation = teamLineups.formation;
                    team.Coach = new Coach
                    {
                        Id = teamLineups.manager?.id,
                        CountryCode = teamLineups.manager?.country_code,
                        Nationality = teamLineups.manager?.nationality,
                        Name = teamLineups.manager?.name
                    };

                    team.Players = teamLineups.starting_lineup.Select(pl => new Player
                    {
                        Id = pl.id,
                        Name = pl.name,
                        Order = pl.order,
                        Position = Enumeration.FromDisplayName<Position>(pl.position),
                        Type = Enumeration.FromDisplayName<PlayerType>(pl.type),
                        JerseyNumber = pl.jersey_number,
                    });

                    team.Substitutions = teamLineups.substitutes.Select(pl => new Player
                    {
                        Id = pl.id,
                        Name = pl.name,
                        Type = Enumeration.FromDisplayName<PlayerType>(pl.type),
                        JerseyNumber = pl.jersey_number
                    });
                }
            }
        }
    }
}