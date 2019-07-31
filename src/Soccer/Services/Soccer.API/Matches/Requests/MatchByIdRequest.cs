namespace Soccer.API.Matches.Requests
{
    using System;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class MatchByIdRequest : IRequest<Match>
    {
        public MatchByIdRequest(string id, string language)
        {
            Id = id;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string Id { get; }

        public Language Language { get; }
    }
}