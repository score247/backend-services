using System.Collections.Generic;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    public class TeamSchedules
    {
        public TeamDto team { get; set; }

        public IEnumerable<SportEventDto> schedule { get; set; }
    }
}