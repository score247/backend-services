using System.Collections.Generic;
using Newtonsoft.Json;
using Soccer.DataProviders.SportRadar._Shared.Extensions;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    public class TeamHeadToHeadsDto
    {
        public IEnumerable<Team> teams { get; set; }

        [JsonConverter(typeof(IgnoreUnexpectedArraysConverter<LastMeetings>))]
        public LastMeetings last_meetings { get; set; }

        public IEnumerable<NextMeeting> next_meetings { get; set; }
    }

    public class LastMeetings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ResultDto> results { get; set; }
    }

    public class NextMeeting
    {
        public SportEventDto sport_event { get; set; }
    }
}