namespace Soccer.API.Matches.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public class MatchesByDateRequest : IRequest<IEnumerable<MatchSummary>>
    {
        public MatchesByDateRequest(DateTimeOffset from, DateTimeOffset to, string language)
        {
            From = from;
            To = to;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public DateTimeOffset From { get; }

        public DateTimeOffset To { get; }

        public Language Language { get; }
    }
}