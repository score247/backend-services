namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Leagues.Models;
    using Soccer.DataProviders.SportRadar.Leagues.Dtos;

    public static class LeagueMapper
    {
        public static League MapLeague(TournamentDto tournament)
            => new League
            {
                Id = tournament?.id,
                Name = tournament?.name,
                Category = new LeagueCategory
                {
                    Id = tournament?.category?.id,
                    Name = tournament?.category?.name,
                    CountryCode = tournament?.category?.country_code
                }
            };

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