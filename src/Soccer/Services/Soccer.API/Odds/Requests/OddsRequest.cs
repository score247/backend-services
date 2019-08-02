namespace Soccer.API.Odds.Requests
{
    using MediatR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;

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
            Languge = Enumeration.FromDisplayName<Language>(language);
            Format = format;
        }

        public string MatchId { get; }

        public int BetTypeId { get; }

        public Language Languge { get; }

        public string Format { get; }
    }
}