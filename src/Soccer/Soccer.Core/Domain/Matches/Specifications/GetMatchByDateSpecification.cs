namespace Soccer.Core.Domain.Matches.Specifications
{
    using System;
    using Score247.Shared.Base;
    using Soccer.Core.Domain.Matches.Entities;

    public class GetMatchByDateSpecification : Specification<MatchEntity>
    {
        public GetMatchByDateSpecification(int sportId, DateTime from, DateTime to, string language, int numberOfMatches)
            : base(
                m => m.SportId == sportId
                && m.Language.Equals(language, StringComparison.OrdinalIgnoreCase)
                && m.EventDate >= from
                && m.EventDate <= to)
        {
            ApplyPaging(0, numberOfMatches);
            ApplyOrderBy(m => m.EventDate);
        }
    }
}