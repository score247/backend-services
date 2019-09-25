namespace Soccer.API.Matches.Requests
{
    using MediatR;
    using Soccer.Core.Shared.Enumerations;

    public class LiveMatchCountRequest : IRequest<int>
    {
        public Language Language => Language.en_US;
    }
}