namespace Soccer.EventProcessors.Leagues
{
    using Soccer.Core.Matches.Models;
    using Soccer.EventProcessors._Shared.Filters;
    using System;
    using System.Collections.Generic;

    public class LeagueFilter : IFilter<IEnumerable<Match>>, IFilter<Match>
    {
        public IEnumerable<Match> Filter(IEnumerable<Match> data)
        {
            throw new NotImplementedException();
        }

        public Match Filter(Match data)
        {
            throw new NotImplementedException();
        }
    }
}
