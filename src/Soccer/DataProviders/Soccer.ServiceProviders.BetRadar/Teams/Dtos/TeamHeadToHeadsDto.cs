using System.Collections.Generic;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    public class TeamHeadToHeadsDto
    {
        public IEnumerable<Team> teams { get; set; }

        public LastMeetings last_meetings { get; set; }

        public IEnumerable<NextMeeting> next_meetings { get; set; }
    }

    public class LastMeetings
    {
        public IEnumerable<ResultDto> results { get; set; }
    }

    public class NextMeeting
    {
        public SportEventDto sport_event { get; set; }
    }
}