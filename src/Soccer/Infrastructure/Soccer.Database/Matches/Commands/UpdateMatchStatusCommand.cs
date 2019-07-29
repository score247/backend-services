namespace Soccer.Database.Matches.Commands
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateMatchResultCommand : NonQueryCommand
    {
        public UpdateMatchResultCommand(string matchId, MatchResult result, string language)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchResult = JsonConvert.SerializeObject(
                result,
                new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            Language = language;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string Language { get; }

        public string MatchResult { get; }

        public override string GetSettingKey() => "Score247_UpdateMatchResult";

        public override bool IsValid() =>
            !string.IsNullOrEmpty(MatchId) &&
            !string.IsNullOrEmpty(Language) &&
            !string.IsNullOrEmpty(MatchResult);
    }
}
