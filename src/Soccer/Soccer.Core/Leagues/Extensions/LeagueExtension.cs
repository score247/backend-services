using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Score247.Shared.Extensions;
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
        private const char spaceChar =  ' ';
        private const int second = 2;
        private const string womenPattern = @"\b(women|w|womens)\b";
        private const string underPattern = @"\b(u[0-9]+)\b";
        private const string youthPattern = @"\b(youth)\b";
        private const string womenPostfix = "Women";

        public static void UpdateMajorLeagueInfo(this League league, IEnumerable<League> majorLeagues)
        {
            var majorLeague = majorLeagues.FirstOrDefault(l => l.Id == league.Id);

            if (majorLeague != null)
            {
                league.UpdateLeague(majorLeague.CountryCode, majorLeague.IsInternational, majorLeague.Order, majorLeague.Region, majorLeague.HasGroups);
            }
        }

        public static string MapLeagueGroupName(this League league, LeagueRound leagueRound, Language language)
        {
            var defaultLeagueName = league?.Name ?? string.Empty;

            if (league == null)
            {
                return defaultLeagueName;
            }

            var leagueGroupName = BuildLeagueGroupName(league, leagueRound);

            return string.IsNullOrWhiteSpace(league.CountryName)
                ? $"{leagueGroupName}".TrimStart()
                : $"{league.CountryName} {leagueGroupName}".TrimStart();
        }

        private static string MapLeagueName(League league) 
        {
            var defaultLeagueName = league?.Name ?? string.Empty;

            if (league == null)
            {
                return defaultLeagueName;
            }

            foreach (var builder in leagueNameBuilders)
            {
                var leagueName = builder(league);

                if (!string.IsNullOrWhiteSpace(leagueName))
                {
                    return leagueName;
                }
            }

            return defaultLeagueName;
        }

        public static League MapCountryAndLeagueName(this League league)
        {
            league.UpdateCountryAndLeagueName(MapLeagueName(league), BuildCountryName(league));

            return league;
        }
     
        private static readonly List<Func<League, string>> leagueNameBuilders =
            new List<Func<League, string>>
            {
                LeagueNameRule3Builder,
                LeagueNameRule2Builder
            };

        private static string ExtractGroupName(League league, string groupName)
            => groupName.Equals(league.Name, StringComparison.InvariantCultureIgnoreCase)
                ? string.Empty
                : groupName[groupName.Length - 1].ToString().ToUpperInvariant();

        private static string BuildCountryName(League league)
        {
            if (league.IsInternational || string.IsNullOrWhiteSpace(league.CountryName))
            {
                return string.Empty;
            }

            var countryNameHasTwoNames = league.CountryName.Any(ch => ch == commaChar);

            if (countryNameHasTwoNames)
            {
#pragma warning disable S1226 // Method parameters, caught exceptions and foreach variables' initial values should not be ignored
                return $"{league.CountryName.Substring(0, league.CountryName.IndexOf(commaChar))}".Trim();
#pragma warning restore S1226 // Method parameters, caught exceptions and foreach variables' initial values should not be ignored
            }

            return $"{league.CountryName}";
        }

        private static string LeagueNameRule2Builder(
            League league)
        {
            var numOfComma = league.Name.Count(nameChar => nameChar == commaChar);

            if (numOfComma == 1)
            {
                return league.Name.Replace(commaString, string.Empty);
            }

            return string.Empty;
        }

        private static string LeagueNameRule3Builder(
            League league)
        {
            var numOfComma = league.Name.Count(ch => ch == commaChar);
            const int twoCommas = 2;

            if (numOfComma == twoCommas)
            {
                var words = league.Name.Split(commaChar);

                return string.Join(string.Empty, words[0], words[second], words[1]);
            }

            return string.Empty;
        }

        private static string BuildLeagueGroupName(League league, LeagueRound leagueRound)
        {
            var formatGroupName = BuildPhaseGroupAndRoundName(league, leagueRound);
          
            return string.IsNullOrWhiteSpace(formatGroupName) 
                ? league.Name 
                : $"{league.Name}{termsplit} {formatGroupName}";
        }

        public static string BuildPhaseGroupAndRoundName(this League league, LeagueRound leagueRound) 
        {
            if (leagueRound == null)
            {
                return string.Empty;
            }

            if (leagueRound.Type == LeagueRoundType.CupRound
                        || leagueRound.Type == LeagueRoundType.QualifierRound)
            {
                var formatPhase = FormatPhaseNotPlayOffs(leagueRound);

                return $"{formatPhase}{leagueRound.Name?.Replace(underscore, space)}";
            }

            if (leagueRound.Type == LeagueRoundType.GroupRound)
            {
                return FormatGroupName(league, leagueRound.Group);
            }

            return string.Empty;
        }

        private static string FormatPhaseNotPlayOffs(LeagueRound leagueRound)
       => string.IsNullOrWhiteSpace(leagueRound.Phase) || IsPlayOffs(leagueRound.Phase)
               ? string.Empty
               : $"{leagueRound.Phase.Replace(underscore, space)}{ termsplit} ";

        private static string FormatGroupName(League league, string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return string.Empty;
            }

            var convertedGroupName = groupName.Length == 1
                    ? groupName.ToUpperInvariant()
                    : ExtractGroupName(league, groupName);

            // Should multiple languages here
            convertedGroupName = string.IsNullOrWhiteSpace(convertedGroupName) || (IsLeagueNameContainsGroup(league.Name, groupName))
                ? string.Empty
                : $"Group {convertedGroupName}";

            return convertedGroupName;
        }

        private static bool IsLeagueNameContainsGroup(string leagueName, string groupName) 
        => leagueName
            .Split(spaceChar)
            .Any(name => name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        

        private static bool IsPlayOffs(string phase) => phase.Equals(playoffs, StringComparison.InvariantCultureIgnoreCase);

        public static string GetTeamNamePostfix(this League league)
        {
            var postfix = string.Empty;

            var womenMatches = Regex.Match(league.Name, womenPattern, RegexOptions.IgnoreCase);
            var underMatches = Regex.Match(league.Name, underPattern, RegexOptions.IgnoreCase);
            var youthMatches = Regex.Match(league.Name, youthPattern, RegexOptions.IgnoreCase);

            if (underMatches.Success)
            {
                postfix += underMatches.Value.ToUpperInvariant();
            }

            if (youthMatches.Success)
            {
                postfix += youthMatches.Value.ToUpperFirstCharInvariant();
            }

            if (womenMatches.Success)
            {
                postfix = string.IsNullOrWhiteSpace(postfix) ? womenPostfix : $"{postfix} {womenPostfix}";
            }

            return postfix;
        }

#pragma warning restore S1172 // Unused method parameters should be removed
    }
}