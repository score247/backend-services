namespace Soccer.API.Matches.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class MatchesByDateRequest : IRequest<IEnumerable<Match>>
    {
        public MatchesByDateRequest(DateTime from, DateTime to, string language, TimeSpan clientTimeOffset)
        {
            From = from;
            To = to;
            Language = Enumeration.FromDisplayName<Language>(language);
            ClientTimeOffset = clientTimeOffset;
        }

        public DateTime From { get; }

        public DateTime To { get; }

        public Language Language { get; }

        public TimeSpan ClientTimeOffset { get; }
    }
}