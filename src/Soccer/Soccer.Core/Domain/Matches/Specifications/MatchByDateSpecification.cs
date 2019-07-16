namespace Soccer.Core.Domain.Matches.Specifications
{
    using System;
    using Soccer.Core.Base;
    using Soccer.Core.Domain.Matches.Entities;

    public class MatchByDateSpecification : Specification<MatchEntity>
    {
        public MatchByDateSpecification(int sportId, DateTime from, DateTime to, string language)
            : base(
                m => m.SportId == sportId
                && m.Language.Equals(language, StringComparison.OrdinalIgnoreCase)
                && m.EventDate >= from
                && m.EventDate <= to)
        {
        }
    }
}