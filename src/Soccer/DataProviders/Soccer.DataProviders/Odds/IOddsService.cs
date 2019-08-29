namespace Soccer.DataProviders.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Odds.Models;

    public interface IOddsService
    {
        Task<IEnumerable<MatchOdds>> GetOdds();

        Task<IEnumerable<MatchOdds>> GetOddsChange(int minuteInterval);

        Task<MatchOdds> GetOdds(string matchId, DateTimeOffset lastUpdated);
    }
}