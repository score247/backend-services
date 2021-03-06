using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues.Requests
{
    public class MatchesByLeagueRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public MatchesByLeagueRequest(string leagueId, string seasonId, string leagueGroupName, string language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            LeagueGroupName = leagueGroupName;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string LeagueId { get; }

        public string SeasonId { get; }

        public string LeagueGroupName { get; }

        public Language Language { get; }
    }
}