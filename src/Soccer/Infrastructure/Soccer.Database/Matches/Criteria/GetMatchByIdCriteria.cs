using System;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchByIdCriteria : CustomCriteria
    {
        public GetMatchByIdCriteria(
            string id,
            Language language,
            DateTimeOffset eventDate = default) : base(eventDate)
        {
            Id = id;
            Language = language.DisplayName;
        }

        public string Id { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetById".GetCorrespondingKey(EventDate);

        public override bool IsValid() => !string.IsNullOrEmpty(Id);
    }
}