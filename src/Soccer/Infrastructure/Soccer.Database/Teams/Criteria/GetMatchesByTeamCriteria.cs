using System;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Teams.Criteria
{
    public class GetMatchesByTeamCriteria : CustomCriteria
    {
        public GetMatchesByTeamCriteria(string teamId, Language language, DateTimeOffset eventDate = default) : base(eventDate)
        {
            TeamId = teamId;
            Language = language.DisplayName;
            SportId = Sport.Soccer.Value;
        }

        public byte SportId { get; }

        public string TeamId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Team_GetMatches".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(TeamId);
    }
}
