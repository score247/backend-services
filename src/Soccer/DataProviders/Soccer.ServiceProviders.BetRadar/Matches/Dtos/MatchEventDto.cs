namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using Newtonsoft.Json;

    public class MatchEventDto
    {
        public MatchEventPayLoadDto payload { get; set; }

        public MatchEventMetadataDto metadata { get; set; }
    }

    public class MatchEventPayLoadDto
    {
        public SportEventStatusDto sport_event_status { get; set; }

        [JsonProperty(PropertyName = "event")]
        public TimelineDto timeline { get; set; }
    }

    public class MatchEventMetadataDto
    {
        public string tournament_id { get; set; }

        public string season_id { get; set; }

        public string sport_event_id { get; set; }

        public string sport_id { get; set; }
    }
}