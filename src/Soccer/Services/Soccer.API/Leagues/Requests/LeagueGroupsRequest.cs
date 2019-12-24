using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues.Requests
{
    public class LeagueGroupsRequest : IRequest<IEnumerable<LeagueGroupState>>
    {
        public LeagueGroupsRequest(string leagueId, string seasonId, string language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string LeagueId { get; }

        public string SeasonId { get; }

        public Language Language { get; }
    }
}