using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Teams.Requests
{
    public class MatchesByTeamRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public MatchesByTeamRequest(string teamId, string language)
        {
            TeamId = teamId;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string TeamId { get; }

        public Language Language { get; }
    }
}
