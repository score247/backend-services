using System;
using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.Extensions
{
    public static class LeagueExtension
    {
        private const string playoffs = "Playoffs";
        private const char commaChar = ',';
        private const string commaString = ",";
        private const string termsplit = "::";
        private const string underscore = "_";
        private const string space = " ";
        private const int second = 2;

        public static void UpdateMajorLeagueInfo(this League league, IEnumerable<League> majorLeagues)
        {
            var majorLeague = majorLeagues.FirstOrDefault(l => l.Id == league.Id);

            if (majorLeague != null)
            {
                league.UpdateLeague(majorLeague.CountryCode, majorLeague.IsInternational, majorLeague.Order, majorLeague.Region);
            }
        }

        public static string MapLeagueGroupName(this League league, LeagueRound leagueRound, Language language)
        {
            var defaultLeagueName = league?.Name ?? string.Empty;

            if (league == null)
            {
                return defaultLeagueName;
            }

            if (leagueRound == null)
            {
                return LeagueNameRule0Builder(league, language);
            }

            foreach (var builder in leagueNameBuilders)
            {
                var leagueName = builder(league, leagueRound, language);

                if (!string.IsNullOrWhiteSpace(leagueName))
                {
                    return CombinePlayoffsInfomration(leagueRound, leagueName);
                }
            }

            return defaultLeagueName;
        }

        // Please refer to wiki to see rule list wiki/Scores_-_Show_League_name_by_rule#LeagueNameRule
        private static readonly List<Func<League, LeagueRound, Language, string>> leagueNameBuilders =
            new List<Func<League, LeagueRound, Language, string>>
            {
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
                || leagueRound.Type == LeagueRoundType.QualifierRound)
            {
                var formatPhase = FormatPhaseNotPlayOffs(leagueRound);

                return $"{BuildLeagueWithCountryName(league)}{termsplit} {formatPhase}{leagueRound.Name?.Replace(underscore, space)}";
            }

            return LeagueNameRule1GroupBuilder(league, leagueRound, language);
        }

        private static string FormatPhaseNotPlayOffs(LeagueRound leagueRound)
        => string.IsNullOrWhiteSpace(leagueRound.Phase) || IsPlayOffs(leagueRound.Phase)
                ? string.Empty
                : $"{leagueRound.Phase.Replace(underscore, space)}{ termsplit} ";

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
            => groupName.Equals(league.Name, StringComparison.InvariantCultureIgnoreCase)
                ? string.Empty
                : groupName[groupName.Length - 1].ToString().ToUpperInvariant();

        private static string BuildLeagueWithCountryName(League league, string leagueName = null)
        {
            var countryName = league.CountryName;
            if (league.IsInternational)
            {
                if (leagueName == null)
                {
                    return league.Name;
                }
                else
                {
                    countryName = string.Empty;
                }
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

            leagueName = string.IsNullOrWhiteSpace(leagueName) ? league.Name : leagueName;

            return $"{countryName} {leagueName}".TrimStart();
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
                var words = league.Name.Split(commaChar);

                return BuildLeagueWithCountryName(
                    league,
                    string.Join(termsplit, words[0], words[second], words[1]));
            }

            return string.Empty;
        }

        private static string CombinePlayoffsInfomration(
            LeagueRound leagueRound,
            string leagueName)
            => (leagueRound?.Phase != null && IsPlayOffs(leagueRound.Phase))
                ? $"{leagueName}{termsplit} {playoffs}"
                : leagueName;

        private static string LeagueNameRule0Builder(
            League league,
            Language language)
        {
            return league.Name.Replace(commaString, string.Empty);
        }

        private static bool IsPlayOffs(string phase) => phase.Equals(playoffs, StringComparison.InvariantCultureIgnoreCase);

#pragma warning restore S1172 // Unused method parameters should be removed
    }
}