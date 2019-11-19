using System;

namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    using Dtos;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Shared.Enumerations;

    public static class LeagueMapper
    {
        public static League MapLeague(TournamentDto tournament, string region)
        {
            if (tournament == null)
            {
                return null;
            }

            var isInternationalLeague = string.IsNullOrWhiteSpace(tournament.category?.country_code);

            return new League(
                tournament.id,
                tournament.name,
                0,
                tournament.category?.id,
                tournament.category?.name,
                tournament.category?.country_code ?? "",
                isInternationalLeague,
                region,
                tournament.current_season?.id ?? "");
        }

        public static LeagueRound MapLeagueRound(TournamentRoundDto tournamentRound)
        {
            if (tournamentRound == null)
            {
                return null;
            }

            var leagueRound = new LeagueRound(
                Enumeration.FromDisplayName<LeagueRoundType>(tournamentRound.type),
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
    }
}