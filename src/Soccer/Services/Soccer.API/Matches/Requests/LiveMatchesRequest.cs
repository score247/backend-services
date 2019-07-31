﻿namespace Soccer.API.Matches.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

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