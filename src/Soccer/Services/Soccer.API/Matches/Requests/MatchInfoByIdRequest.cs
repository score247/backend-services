namespace Soccer.API.Matches.Requests
{
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.API.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public class MatchInfoByIdRequest : IRequest<MatchInfo>
    {
        public MatchInfoByIdRequest(string id, string language)
        {
            Id = id;
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public string Id { get; }

        public Language Language { get; }
    }
}