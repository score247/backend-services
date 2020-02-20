using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Teams.Requests
{
    public class SearchTeamByNameRequest : IRequest<IEnumerable<TeamProfile>>
    {
        public SearchTeamByNameRequest(string keyword, string language)
        {
            Keyword = keyword;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string Keyword { get; }

        public Language Language { get; }
    }
}
