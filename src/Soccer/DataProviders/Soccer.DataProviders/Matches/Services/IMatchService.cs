namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public interface IMatchService
    {
        Task<IReadOnlyList<Match>> GetPreMatches(DateTime utcFrom, DateTime utcTo, Language language);

        Task<IList<Match>> GetPostMatches(DateTime utcFrom, DateTime utcTo, Language language);

        Task<IReadOnlyList<Match>> GetLiveMatches(Language language);
    }
}