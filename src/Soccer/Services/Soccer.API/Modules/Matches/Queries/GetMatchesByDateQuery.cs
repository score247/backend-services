namespace Soccer.API.Modules.Matches.Queries
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Soccer.Core.Domain.Matches.Models;

    public class GetMatchesByDateQuery : IRequest<IEnumerable<Match>>
    {
        public GetMatchesByDateQuery(DateTime from, DateTime to, string language, TimeSpan clientTimeOffset)
        {
            From = from;
            To = to;
            Language = language;
            ClientTimeOffset = clientTimeOffset;
        }

        public DateTime From { get; }

        public DateTime To { get; }

        public string Language { get; }

        public TimeSpan ClientTimeOffset { get; }
    }
}