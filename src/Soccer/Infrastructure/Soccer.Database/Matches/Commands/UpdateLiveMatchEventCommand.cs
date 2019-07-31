namespace Soccer.Database.Matches.Commands
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateLiveMatchEventCommand : NonQueryCommand
    {
        public UpdateLiveMatchEventCommand(string matchId, MatchEvent matchEvent)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchEvent = JsonConvert.SerializeObject(
                matchEvent,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
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