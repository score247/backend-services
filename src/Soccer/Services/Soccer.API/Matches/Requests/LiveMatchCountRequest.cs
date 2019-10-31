namespace Soccer.API.Matches.Requests
{
    using MediatR;
    using Soccer.Core.Shared.Enumerations;

    public class LiveMatchCountRequest : IRequest<int>
    {
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public Language Language => Language.en_US;
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    }
}