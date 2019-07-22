namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Domain.Matches.Models;

    public interface IMatchService
    {
        Task<IReadOnlyList<Match>> GetPreMatches(DateTime utcFrom, DateTime utcTo, string language);

        Task<IReadOnlyList<Match>> GetPostMatches(DateTime utcFrom, DateTime utcTo, string language);
    }
}