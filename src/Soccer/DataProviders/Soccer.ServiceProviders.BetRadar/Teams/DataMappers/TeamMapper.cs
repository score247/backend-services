namespace Soccer.DataProviders.SportRadar.Teams.DataMappers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Soccer.Core.Teams.Models;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;
    using Soccer.DataProviders.SportRadar.Teams.Dtos;

    public static class TeamMapper
    {
        private const string Home = "home";

        public static List<Team> MapTeams(SportEventDto sportEventDto)
        {
            var teams = new List<Team>();

            foreach (var competitor in sportEventDto.competitors)
            {
                teams.Add(new Team(
                    competitor.id,
                    competitor.name,
                    competitor.country,
                    competitor.country_code,
                    string.Empty,
                    string.Compare(competitor.qualifier, Home, true, CultureInfo.InvariantCulture) == 0,
                    new TeamStatistic(),
                    competitor.abbreviation)
                );
            }

            return teams.OrderBy(team => team.IsHome ? 0 : 1).ToList();
        }

        public static TeamProfile MapTeam(TeamDto teamDto, string postfix)
        => new TeamProfile(
            teamDto.id, 
            string.IsNullOrWhiteSpace(postfix) ? teamDto.name : $"{teamDto.name} {postfix}", 
            teamDto.country, 
            teamDto.country_code, 
            teamDto.abbreviation);
    }
}