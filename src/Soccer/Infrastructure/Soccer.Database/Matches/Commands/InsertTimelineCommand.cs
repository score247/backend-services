namespace Soccer.Database.Matches.Commands
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using Soccer.Core.Matches.Models;

    public class InsertTimelineCommand : NonQueryCommand
    {
        public InsertTimelineCommand(string matchId, Timeline timeline)
        {
            MatchId = matchId;
            Timeline = JsonConvert.SerializeObject(
                timeline,
                new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        public string MatchId { get; }

        public string Timeline { get; set; }

        public override string GetSettingKey() => "Score247_InsertTimeline";

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId) && !string.IsNullOrEmpty(Timeline);
    }
}