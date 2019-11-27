namespace Soccer.API.Matches.Requests
{
    using System;
    using MediatR;
    using Soccer.API.Matches.Models;

    public class MatchStatisticRequest : IRequest<MatchStatistic>
    {
        public MatchStatisticRequest(string id, DateTimeOffset eventDate)
        {
            Id = id;
            EventDate = eventDate;
        }

        public string Id { get; }

        public DateTimeOffset EventDate { get; }
    }
}