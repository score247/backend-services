using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Teams.Requests
{
    public class TrendingTeamsRequest : IRequest<IEnumerable<TeamProfile>>
    {
        public TrendingTeamsRequest(string language)
        {
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public Language Language { get; }
    }
}
