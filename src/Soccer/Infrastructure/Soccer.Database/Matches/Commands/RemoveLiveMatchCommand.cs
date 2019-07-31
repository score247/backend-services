namespace Soccer.Database.Matches.Commands
{
    using Fanex.Data.Repository;

    public class RemoveLiveMatchCommand : BaseCommand
    {
        public RemoveLiveMatchCommand(string matchId)
        {
            MatchId = matchId;
        }

        public string MatchId { get; }

        public override string GetSettingKey() => "Score247_RemoveLiveMatch";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId);
    }
}