namespace Soccer.API.Modules.Matches.Queries
{
    using System;
    using MediatR;
    using Soccer.Core.Domain.Matches.Models;

    public class GetMatchByIdQuery : IRequest<Match>
    {
        public GetMatchByIdQuery(string id, TimeSpan clientTimeOffset)
        {
            Id = id;
            ClientTimeOffset = clientTimeOffset;
        }

        public string Id { get; }

        public TimeSpan ClientTimeOffset { get; }
    }
}