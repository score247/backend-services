using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches.Requests
{
    public class MatchesByIdsRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public MatchesByIdsRequest(string[] ids, string language)
        {
            Ids = ids;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string[] Ids { get; }

        public Language Language { get; }
    }
}
