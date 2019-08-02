namespace Soccer.API.Odds.Requests
{
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;

    public class OddsMovementRequest : IRequest<MatchOddsMovement>
    {
        public OddsMovementRequest(
            string matchId,
            int betTypeId,
            string bookmakerId,
            string language = "en-US",
            string format = "dec")
        {
            BookmakerId = bookmakerId;
            MatchId = matchId;
            BetTypeId = betTypeId;
            Languge = Enumeration.FromDisplayName<Language>(language);
            Format = format;
        }

        public string MatchId { get; }

        public int BetTypeId { get; }

        public Language Languge { get; }

        public string Format { get; }

        public string BookmakerId { get; }
    }
}