using System;
using System.Collections.Generic;
using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;

namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    public static class LeagueMapper
    {
        private const string playoffs = "Playoffs";
        private const char commaChar = ',';
        private const string commaString = ",";
        private const string termsplit = "::";
        private const string underscore = "_";
        private const string space = " ";

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
                tournament.category?.country_code ?? string.Empty,
                isInternationalLeague,
                region,
                tournament.current_season?.id ?? string.Empty);
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

        public static string MapLeagueGroupName(League league, LeagueRound leagueRound, Language language)
        {
            if (league == null && leagueRound == null)
            {
                return string.Empty;
            }

            foreach (var builder in leagueNameBuilders)
            {
                var leagueName = builder(league, leagueRound, language);

                if (!string.IsNullOrWhiteSpace(leagueName))
                {
                    return leagueName;
                }
            }

            return string.Empty;
        }

        // Please refer to wiki to see rule list wiki/Scores_-_Show_League_name_by_rule#LeagueNameRule
        private static readonly List<Func<League, LeagueRound, Language, string>> leagueNameBuilders =
            new List<Func<League, LeagueRound, Language, string>>
            {
                LeagueNameRule4Builder,
                LeagueNameRule3Builder,
                LeagueNameRule2Builder,
                LeagueNameRule1Builder
            };

        private static string LeagueNameRule1Builder(
           League league,
           LeagueRound leagueRound,
           Language language)
        {
            if (leagueRound.Type == LeagueRoundType.CupRound
                && !string.IsNullOrWhiteSpace(leagueRound?.Name))
            {
                return $"{BuildLeagueWithCountryName(league)}{termsplit} {leagueRound.Name?.Replace(underscore, space)}";
            }

            return LeagueNameRule1GroupBuilder(league, leagueRound, language);
        }

        private static string LeagueNameRule1GroupBuilder(
            League league,
            LeagueRound leagueRound,
#pragma warning disable S1172 // Unused method parameters should be removed
            Language language)
        {
            var groupName = leagueRound?.Group;
            var convertedGroupName = string.Empty;

            if (leagueRound?.Type == LeagueRoundType.GroupRound)
            {
                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    convertedGroupName =
                        groupName.Length == 1
                            ? groupName.ToUpperInvariant()
                            : ExtractGroupName(league, groupName);
                }

                // Should multiple languages here
                convertedGroupName = string.IsNullOrWhiteSpace(convertedGroupName)
                    ? string.Empty
                    : $"{termsplit} Group {convertedGroupName}";
            }

            return BuildLeagueWithCountryName(league) + convertedGroupName;
        }

        private static string ExtractGroupName(League league, string groupName)
            => groupName.Equals(league.Name)
                ? string.Empty
                : groupName[groupName.Length - 1].ToString().ToUpperInvariant();

        private static string BuildLeagueWithCountryName(League league, string countryName = null)
        {
            if (league.IsInternational)
            {
                return league.Name;
            }

            if (!string.IsNullOrWhiteSpace(league.CountryName))
            {
                var countryNameHasTwoNames = league.CountryName.Any(ch => ch == commaChar);

                if (countryNameHasTwoNames)
                {
#pragma warning disable S1226 // Method parameters, caught exceptions and foreach variables' initial values should not be ignored
                    countryName = league.CountryName.Substring(0, league.CountryName.IndexOf(commaChar));
#pragma warning restore S1226 // Method parameters, caught exceptions and foreach variables' initial values should not be ignored
                }
            }

            return $"{countryName ?? league.CountryName} {league.Name}";
        }

        private static string LeagueNameRule2Builder(
            League league,
            LeagueRound leagueRound,
            Language language)
        {
            var numOfComma = league.Name.Count(nameChar => nameChar == commaChar);

            if (numOfComma == 1)
            {
                return BuildLeagueWithCountryName(league).Replace(commaString, termsplit);
            }

            return string.Empty;
        }

        private static string LeagueNameRule3Builder(
            League league,
            LeagueRound leagueRound,
            Language language)
        {
            var numOfComma = league.Name.Count(ch => ch == commaChar);
            const int twoCommas = 2;

            if (numOfComma == twoCommas)
            {
                return BuildLeagueWithCountryName(league).Replace(commaString, termsplit);
            }

            return string.Empty;
        }

        private static string LeagueNameRule4Builder(
            League league,
            LeagueRound leagueRound,
            Language language)
        {
            if (leagueRound?.Phase != null)
            {
                var isPlayOffs = leagueRound.Phase.Equals(playoffs, StringComparison.InvariantCultureIgnoreCase);

                if (isPlayOffs)
                {
                    var leagueName = LeagueNameRule1Builder(league, leagueRound, language);

                    return $"{leagueName}{termsplit} {playoffs}";
                }
            }

            return string.Empty;
        }
#pragma warning restore S1172 // Unused method parameters should be removed
    }
}