using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues.Requests
{
    public class LeagueGroupsRequest : IRequest<IEnumerable<LeagueGroupState>>
    {
        public LeagueGroupsRequest(string leagueId, string language)
        {
            LeagueId = leagueId;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string LeagueId { get; }
        public Language Language { get; }
    }
}