using System;
using System.Collections.Generic;
using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Soccer.DataProviders.SportRadar.Teams.DataMappers;

namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    public static class LeagueMapper
    {
        public static League MapLeague(TournamentDetailDto tournamentDetail, string region, Language language)
        {
            var league = MapLeague(tournamentDetail.tournament, region);
            league.UpdateHasGroups(tournamentDetail.groups?.Count() > 1);
            league.UpdateTeams(MapTeams(tournamentDetail.groups, league.GetTeamNamePostfix()));

            return league;
        }

        public static League MapLeague(TournamentDto tournament, string region)
        {
            if (tournament == null)
            {
                return null;
            }

            var isInternationalLeague = string.IsNullOrWhiteSpace(tournament.category?.country_code);

            LeagueSeasonDates leagueSeasonDates = null;

            if (tournament.current_season != null)
            {
                leagueSeasonDates = new LeagueSeasonDates(
                    tournament.current_season.start_date,
                    tournament.current_season.end_date);
            }

            var league = new League(
                    tournament.id,
                    tournament.name,
                    0,
                    tournament.category?.id,
                    tournament.category?.name,
                    tournament.category?.country_code ?? string.Empty,
                    isInternationalLeague,
                    region,
                    tournament.current_season?.id ?? string.Empty,
                    leagueSeasonDates,
                    false,
                    string.Empty)
                .MapCountryAndLeagueName();

            return league;
        }

        public static LeagueRound MapLeagueRound(TournamentRoundDto tournamentRound)
        {
            if (tournamentRound == null)
            {
                return null;
            }

            var leagueRound = new LeagueRound(
                string.IsNullOrWhiteSpace(tournamentRound.type)
                    ? LeagueRoundType.UnknownRound
                    : Enumeration.FromDisplayName<LeagueRoundType>(tournamentRound.type),
                tournamentRound.name,
                tournamentRound.number,
                tournamentRound.phase,
                tournamentRound.group);

            return leagueRound;
        }

        public static LeagueSeason MapLeagueSeason(SeasonDto seasonDto)
        {
            if (seasonDto == null)
            {
                return null;
            }

            var leagueSeason = new LeagueSeason(
                seasonDto.id,
                seasonDto.name,
                Convert.ToDateTime(seasonDto.start_date),
                Convert.ToDateTime(seasonDto.end_date),
                seasonDto.year,
                seasonDto.tournament_id);

            return leagueSeason;
        }

        private static IList<TeamProfile> MapTeams(IEnumerable<TournamentGroup> groups, string postfix)
        {
            var teams = new List<TeamProfile>();

            if (groups != null)
            {
                var leagueTeams = groups.SelectMany(group => group.teams).ToList();

                foreach (var team in leagueTeams)
                {
                    teams.Add(TeamMapper.MapTeam(team, postfix));
                }
            }

            return teams;
        }
    }
}