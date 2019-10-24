﻿using MediatR;
using Score247.Shared.Enumerations;
using Soccer.API.Matches.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches.Requests
{
    public class MatchLineupsRequest : IRequest<MatchLineups>
    {
        public MatchLineupsRequest(string id, string language)
        {
            Id = id;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string Id { get; }

        public Language Language { get; }
    }
}