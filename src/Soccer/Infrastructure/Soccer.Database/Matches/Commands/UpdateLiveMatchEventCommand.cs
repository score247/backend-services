namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateLiveMatchEventCommand : BaseCommand
    {
        public UpdateLiveMatchEventCommand(string matchId, MatchEvent matchEvent)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchEvent = ToJsonString(matchEvent);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string MatchEvent { get; }

        public override string GetSettingKey() => "Score247_UpdateLiveMatchEvent";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(MatchEvent);
    }
}