using System;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.API.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches.Requests
{
    public class MatchCoverageByIdRequest : IRequest<MatchCoverage>
    {
        public MatchCoverageByIdRequest(string id, string language, DateTimeOffset eventDate = default)
        {
            Id = id;
            Language = Enumeration.FromDisplayName<Language>(language);
            EventDate = eventDate;
        }

        public string Id { get; }

        public Language Language { get; }

        public DateTimeOffset EventDate { get; }
    }
}
