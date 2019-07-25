namespace Soccer.API.Modules.Matches.Requests
{
    using System;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Enumerations;

    public class MatchByIdRequest : IRequest<Match>
    {
        public MatchByIdRequest(string id, TimeSpan clientTimeOffset, string language)
        {
            Id = id;
            ClientTimeOffset = clientTimeOffset;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string Id { get; }

        public TimeSpan ClientTimeOffset { get; }

        public Language Language { get; }
    }
}