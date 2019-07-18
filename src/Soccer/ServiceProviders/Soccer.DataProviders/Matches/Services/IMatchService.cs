namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Domain.Matches.Models;

    public interface IMatchService
    {
        Task<IEnumerable<Match>> GetSchedule(DateTime utcFrom, DateTime utcTo, string language);
    }
}