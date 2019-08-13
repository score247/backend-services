namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;

    public class UpdateLiveMatchTeamRedCardsCommand : BaseCommand
    {
        public UpdateLiveMatchTeamRedCardsCommand(string matchId, bool isHome, int redCards)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            TeamIndex = isHome ? 0 : 1;
            RedCards = redCards;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public int TeamIndex { get; }

        public int RedCards { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateTeamRedCards";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId);
    }
}