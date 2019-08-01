namespace Soccer.API.Odds.Requests
{
    using MediatR;
    using Soccer.Core.Odds.Models;

    public class OddsRequest : IRequest<MatchOdds>
    {
        public OddsRequest(
            string matchId,
            int betTypeId,
            string language = "en-US",
            string format = "dec")
        {
            MatchId = matchId;
            BetTypeId = betTypeId;
            Languge = language;
            Format = format;
        }

        public string MatchId { get; }

        public int BetTypeId { get; }

        public string Languge { get; }

        public string Format { get; }
    }
}