namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public interface IMatchService
    {
        Task<IReadOnlyList<Match>> GetPreMatches(DateTime date, Language language);

        Task<IReadOnlyList<Match>> GetPostMatches(DateTime date, Language language);

        Task<IReadOnlyList<Match>> GetLiveMatches(Language language);
    }
}