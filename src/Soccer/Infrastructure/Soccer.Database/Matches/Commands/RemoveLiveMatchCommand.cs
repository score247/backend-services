namespace Soccer.Database.Matches.Commands
{
    public class RemoveLiveMatchCommand : BaseCommand
    {
        public RemoveLiveMatchCommand(string matchId)
        {
            MatchId = matchId;
        }

        public string MatchId { get; }

        public override string GetSettingKey() => "LiveMatch_RemoveById";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId);
    }
}