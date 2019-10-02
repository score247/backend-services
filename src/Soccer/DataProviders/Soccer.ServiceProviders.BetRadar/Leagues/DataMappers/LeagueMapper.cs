namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.SportRadar.Leagues.Dtos;

    public static class LeagueMapper
    {
        public static League MapLeague(TournamentDto tournament)
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
                tournament.category?.country_code,
                isInternationalLeague);
        }

        public static LeagueRound MapLeagueRound(TournamentRoundDto tournamentRound)
        {
            var leagueRound = new LeagueRound();

            if (tournamentRound != null)
            {
                leagueRound.Type = Enumeration.FromDisplayName<LeagueRoundType>(tournamentRound.type);
                leagueRound.Name = tournamentRound.name;
                leagueRound.Number = tournamentRound.number;
                leagueRound.Phase = tournamentRound.phase;
                leagueRound.Group = tournamentRound.group;
            }

            return leagueRound;
        }
    }
}