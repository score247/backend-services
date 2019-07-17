namespace Soccer.Core.Domain.Matches.Specifications
{
    using System;
    using Score247.Shared.Base;
    using Score247.Shared.Extensions;
    using Soccer.Core.Domain.Matches.Entities;

    public class GetLiveMatchSpecification : Specification<LiveMatchEntity>
    {
        public GetLiveMatchSpecification(int sportId, TimeSpan clientTimeOffset, string language)
            : base(
                m => m.SportId == sportId
                && m.Language.Equals(language, StringComparison.OrdinalIgnoreCase)
                && m.EventDate >= DateTime.Today.ConvertFromLocalToOffset(clientTimeOffset)
                && m.EventDate <= DateTime.Today.EndDay().ConvertFromLocalToOffset(clientTimeOffset))
        {
            ApplyOrderBy(m => m.EventDate);
        }
    }
}