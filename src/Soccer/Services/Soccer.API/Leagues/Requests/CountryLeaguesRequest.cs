using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Leagues.Requests
{
    public class CountryLeaguesRequest : IRequest<IEnumerable<League>>
    {
        public CountryLeaguesRequest(string countryName, string language)
        {
            CountryCode = countryName;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string CountryCode { get; }
        public Language Language { get; }
    }
}