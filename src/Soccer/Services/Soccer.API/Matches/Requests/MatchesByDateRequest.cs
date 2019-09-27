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
        public MatchesByDateRequest(DateTime from, DateTime to, string language)
        {
            From = from;
            To = to;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public DateTime From { get; }

        public DateTime To { get; }

        public Language Language { get; }
    }
}