namespace Soccer.API.Matches.Requests
{
    using MediatR;
    using Soccer.API.Matches.Models;

    public class MatchStatisticRequest : IRequest<MatchStatistic>
    {
        public MatchStatisticRequest(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}