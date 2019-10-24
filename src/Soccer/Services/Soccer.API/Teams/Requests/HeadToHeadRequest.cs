using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Teams.Requests
{
    public class HeadToHeadRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public HeadToHeadRequest(string homeTeamId, string awayTeamId, string language)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public Language Language { get; }
    }
}