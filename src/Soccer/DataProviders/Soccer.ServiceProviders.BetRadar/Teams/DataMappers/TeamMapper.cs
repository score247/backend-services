namespace Soccer.DataProviders.SportRadar.Teams.DataMappers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Soccer.Core.Teams.Models;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public static class TeamMapper
    {
        private const string Home = "home";

        public static List<Team> MapTeams(SportEventDto sportEventDto)
        {
            var teams = new List<Team>();

            foreach (var competitor in sportEventDto.competitors)
            {
                teams.Add(new Team
                {
                    Id = competitor.id,
                    Country = competitor.country,
                    CountryCode = competitor.country_code,
                    Name = competitor.name,
                    Abbreviation = competitor.abbreviation,
                    IsHome = string.Compare(competitor.qualifier, Home, true, CultureInfo.InvariantCulture) == 0,
                    Statistic = new TeamStatistic()
                });
            }

            return teams.OrderBy(team => team.IsHome ? 0 : 1).ToList();
        }


    }
}