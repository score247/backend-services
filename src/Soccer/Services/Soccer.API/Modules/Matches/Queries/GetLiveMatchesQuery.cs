namespace Soccer.API.Modules.Matches.Queries
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Soccer.Core.Domain.Matches.Models;

    public class GetLiveMatchesQuery : IRequest<IEnumerable<Match>>
    {
        public GetLiveMatchesQuery(TimeSpan clientTimeOffset, string language)
        {
            Language = language;
            ClientTimeOffset = clientTimeOffset;
        }

        public string Language { get; }

        public TimeSpan ClientTimeOffset { get; }
    }
}