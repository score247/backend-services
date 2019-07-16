namespace Soccer.API.Modules.Matches.Queries
{
    using System.Collections.Generic;
    using MediatR;
    using Soccer.Core.Domain.Matches.Models;

    public class GetLiveMatchesQuery : IRequest<IEnumerable<Match>>
    {
        public GetLiveMatchesQuery(int sportId, string language)
        {
            SportId = sportId;
            Language = language;
        }

        public int SportId { get; }

        public string Language { get; }
    }
}