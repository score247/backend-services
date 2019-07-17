namespace Soccer.Core.Domain.Matches.Specifications
{
    using System;
    using Score247.Shared.Base;
    using Score247.Shared.Extensions;
    using Soccer.Core.Domain.Matches.Entities;

    public class GetLiveMatchSpecification : Specification<LiveMatchEntity>
    {
        public GetLiveMatchSpecification(int sportId, TimeSpan clientTimeZone, string language)
            : base(
                m => m.SportId == sportId
                && m.Language.Equals(language, StringComparison.OrdinalIgnoreCase)
                && m.EventDate >= DateTime.Today.ConvertFromLocalToTimeZone(clientTimeZone)
                && m.EventDate <= DateTime.Today.AddDays(1).AddSeconds(-1).ConvertFromLocalToTimeZone(clientTimeZone))
        {
            ApplyOrderBy(m => m.EventDate);
        }
    }
}