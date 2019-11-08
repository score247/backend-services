using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues.Requests
{
    public class MajorLeaguesRequest : IRequest<IEnumerable<League>>
    {
        public MajorLeaguesRequest(string language)
        {
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public Language Language { get; }
    }
}