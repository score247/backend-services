namespace Soccer.API.Modules.Matches.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Enumerations;

    public class LiveMatchesRequest : IRequest<IEnumerable<Match>>
    {
        public LiveMatchesRequest(TimeSpan clientTimeOffset, string language)
        {
            Language = Enumeration.FromDisplayName<Language>(language);
            ClientTimeOffset = clientTimeOffset;
        }

        public Language Language { get; }

        public TimeSpan ClientTimeOffset { get; }
    }
}