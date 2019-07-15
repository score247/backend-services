namespace Soccer.API.Modules.Matches.Queries
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Soccer.Core.Domain.Matches;

    public class GetMatchesByDateQuery : IRequest<IEnumerable<Match>>
    {
        public GetMatchesByDateQuery(int sportId, DateTime from, DateTime to, string language, TimeSpan timeSpan)
        {
            SportId = sportId;
            From = from;
            To = to;
            Language = language;
            TimeSpan = timeSpan;
        }

        public int SportId { get; }

        public DateTime From { get; }

        public DateTime To { get; }

        public string Language { get;  }

        public TimeSpan TimeSpan { get;  }
    }
}