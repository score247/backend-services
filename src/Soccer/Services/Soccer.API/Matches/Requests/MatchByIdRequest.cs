namespace Soccer.API.Matches.Requests
{
    using System;
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;

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