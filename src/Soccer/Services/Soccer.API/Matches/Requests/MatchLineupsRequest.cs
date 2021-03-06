using System;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.API.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches.Requests
{
    public class MatchLineupsRequest : IRequest<MatchPitchViewLineups>
    {
        public MatchLineupsRequest(string id, string language, DateTimeOffset eventDate, bool useNewFormat = false)
        {
            Id = id;
            Language = Enumeration.FromDisplayName<Language>(language);
            EventDate = eventDate;
            UseNewFormat = useNewFormat;
        }

        public string Id { get; }

        public Language Language { get; }

        public DateTimeOffset EventDate { get; }

        public bool UseNewFormat { get; }
    }
}