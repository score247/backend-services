﻿namespace Soccer.Core.Domain.Matches.Repositories.Criterias
{
    using Fanex.Data.Repository;
    using Score247.Shared.Enumerations;

    public class GetLiveMatchesQuery : CriteriaBase
    {
        public GetLiveMatchesQuery(string language)
        {
            SportId = int.Parse(Sport.Soccer.Value);
            Language = language;
        }

        public int SportId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Score247_GetLiveMatches";

        public override bool IsValid() => SportId > 0 && !string.IsNullOrEmpty(Language);
    }
}