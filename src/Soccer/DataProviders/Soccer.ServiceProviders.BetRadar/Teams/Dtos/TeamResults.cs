using System.Collections.Generic;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    public class TeamResults
    {
        public TeamDto team { get; set; }

        public IEnumerable<ResultDto> results { get; set; }
    }
}