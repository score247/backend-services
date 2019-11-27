using System;
using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.API.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches.Requests
{
    public class MatchCommentaryByIdRequest : IRequest<IEnumerable<MatchCommentary>>
    {
        public MatchCommentaryByIdRequest(string id, string language, DateTimeOffset eventDate)
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