using System;
using System.Linq;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchesByIdsCriteria : CustomCriteria
    {
        public GetMatchesByIdsCriteria(
            string[] ids,
            Language language,
            DateTimeOffset eventDate = default) : base(eventDate)
        {
            Ids = JsonStringConverter.ToJsonString(ids.Select(x => new { MatchId = x }));
            Language = language.DisplayName;
        }

        public string Ids { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetByIds".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => !string.IsNullOrEmpty(Ids);
    }
}
