namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;

    public class MatchSummary
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public SportEvent sport_event { get; set; }

        public SportEventConditions sport_event_conditions { get; set; }

        public SportEventStatus sport_event_status { get; set; }

        public Statistics statistics { get; set; }
    }

    public class SportEventConditions
    {
        public Referee referee { get; set; }

        public IEnumerable<RefereeAssistants> referee_assistants { get; set; }

        public Venue venue { get; set; }

        public int attendance { get; set; }

        public WeatherInfo weather_info { get; set; }
    }

    public class Referee
    {
        public string id { get; set; }

        public string name { get; set; }

        public string nationality { get; set; }

        public string country_code { get; set; }
    }

    public class WeatherInfo
    {
        public string pitch { get; set; }

        public string weather_conditions { get; set; }
    }

    public class RefereeAssistants
    {
        public string type { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public string nationality { get; set; }

        public string country_code { get; set; }
    }

    public class Venue
    {
        public string id { get; set; }

        public string name { get; set; }

        public int capacity { get; set; }

        public string city_name { get; set; }

        public string country_name { get; set; }

        public string map_coordinates { get; set; }

        public string country_code { get; set; }
    }
}