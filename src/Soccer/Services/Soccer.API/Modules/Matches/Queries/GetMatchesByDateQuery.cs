namespace Soccer.API.Modules.Matches.Queries
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Soccer.Core.Domain.Matches.Models;

    public class GetMatchesByDateQuery : IRequest<IEnumerable<Match>>
    {
        public GetMatchesByDateQuery(int sportId, DateTime from, DateTime to, string language, TimeSpan clientTimeZone)
        {
            SportId = sportId;
            From = from;
            To = to;
            Language = language;
            ClientTimeZone = clientTimeZone;
        }

        public int SportId { get; }

        public DateTime From { get; }

        public DateTime To { get; }

        public string Language { get; }

        public TimeSpan ClientTimeZone { get; }
    }
}