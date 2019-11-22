using System;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetMatchesByLeagueCriteria : CustomCriteria
    {
        private const string SpName = "Match_GetByLeagueId";
        public GetMatchesByLeagueCriteria(
            string leagueId, 
            Language language,
            DateTime eventDate = default) : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            LeagueId = leagueId;
            Language = language.DisplayName;
        }

        public int SportId { get; }

        public string  LeagueId { get; }

        public string Language { get; }

        public override string GetSettingKey() => SpName.GetCorrespondingKey(EventDate);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(LeagueId);
    }
}
