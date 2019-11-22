using System;
using Fanex.Data.Repository;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetMatchesByLeagueCriteria : CriteriaBase
    {
        private const string SpName = "Match_GetByLeagueId";
        public GetMatchesByLeagueCriteria(
            string leagueId, 
            Language language,
            DateTime eventDate = default)
        {
            SportId = Sport.Soccer.Value;
            LeagueId = leagueId;
            Language = language.DisplayName;

            EventDate = eventDate == default ? DateTime.Now : eventDate;
        }

        public int SportId { get; }

        public string  LeagueId { get; }

        public string Language { get; }

        DateTime EventDate { get; }

        public override string GetSettingKey() => SpName.GetCorrespondingKey(EventDate);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(LeagueId);
    }
}
