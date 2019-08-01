namespace Soccer.API.Odds.Requests
{
    using MediatR;
    using Soccer.Core.Odds.Models;

    public class OddsMovementRequest : OddsRequest, IRequest<MatchOddsMovement>
    {
        public OddsMovementRequest(
            string matchId,
            int betTypeId,
            string bookmakerId,
            string language = "en-US",
            string format = "dec") : base(matchId, betTypeId, language, format)
        {
            BookmakerId = bookmakerId;
        }

        public string BookmakerId { get; }
    }
}