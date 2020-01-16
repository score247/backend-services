namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public interface IMatchService
    {
        Task<IReadOnlyList<Match>> GetPreMatches(DateTime date, Language language);

        Task<IReadOnlyList<Match>> GetPostMatches(DateTime date, Language language);

        Task<(IReadOnlyList<string>,IReadOnlyList<Match>)> GetLiveMatches(Language language);

        Task<MatchLineups> GetLineups(string matchId, string region, Language language);
    }
}