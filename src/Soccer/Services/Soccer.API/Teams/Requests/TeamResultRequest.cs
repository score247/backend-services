using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Teams.Requests
{
    public class TeamResultRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public TeamResultRequest(string teamId, string opponentTeamId, string language)
        {
            TeamId = teamId;
            OpponentTeamId = opponentTeamId;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string TeamId { get; }

        public string OpponentTeamId { get; }

        public Language Language { get; }
    }
}