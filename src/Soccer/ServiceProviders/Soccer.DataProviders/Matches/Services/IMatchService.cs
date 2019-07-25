namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Enumerations;

    public interface IMatchService
    {
        Task<IReadOnlyList<Match>> GetPreMatches(DateTime utcFrom, DateTime utcTo, Language language);

        Task<IReadOnlyList<Match>> GetPostMatches(DateTime utcFrom, DateTime utcTo, Language language);
    }
}