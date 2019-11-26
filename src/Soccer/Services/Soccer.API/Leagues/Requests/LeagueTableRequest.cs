using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues.Requests
{
    public class LeagueTableRequest : IRequest<LeagueTable>
    {
        public LeagueTableRequest(
            string leagueId,
            string seasonId,
            string groupName,
            string language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            GroupName = groupName;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string LeagueId { get; }

        public string SeasonId { get; }

        public string GroupName { get; }

        public Language Language { get; }
    }
}