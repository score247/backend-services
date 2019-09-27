namespace Soccer.API.Matches.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public class LiveMatchesRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public LiveMatchesRequest(string language)
        {
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public Language Language { get; }
    }
}