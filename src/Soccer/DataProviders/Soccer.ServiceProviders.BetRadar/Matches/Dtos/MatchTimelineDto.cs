namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Teams.Dtos;

    public class MatchTimelineDto
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public SportEventDto sport_event { get; set; }

        public SportEventStatusDto sport_event_status { get; set; }

        public SportEventConditions sport_event_conditions { get; set; }

        public IEnumerable<TimelineDto> timeline { get; set; }

        public StatisticsDto statistics { get; set; }

        public CoverageInfoDto coverage_info { get; set; }
    }

    public class StatisticsDto
    {
        public IEnumerable<Team> teams { get; set; }
    }

    public class TimelineDto
    {
        public long id { get; set; }

        public string type { get; set; }

        public DateTime time { get; set; }

        public IEnumerable<CommentaryDto> commentaries { get; set; }

        public int period { get; set; }

        public string period_type { get; set; }

        public string period_name { get; set; }

        public int match_time { get; set; }

        public string match_clock { get; set; }

        public string team { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public int home_score { get; set; }

        public int away_score { get; set; }

        public GoalScorerDto goal_scorer { get; set; }

        public AssistDto assist { get; set; }

        public PlayerDto player { get; set; }

        public string outcome { get; set; }

        public PlayerDto player_out { get; set; }

        public PlayerDto player_in { get; set; }

        public string stoppage_time { get; set; }

        public int injury_time_announced { get; set; }

        public string description { get; set; }

        public int? shootout_home_score { get; set; }

        public int? shootout_away_score { get; set; }

        public string status { get; set; }
    }

    public class PlayerDto
    {
        public string id { get; set; }

        public string name { get; set; }
    }

    public class CommentaryDto
    {
        public string text { get; set; }
    }

    public class CoverageInfoDto
    {
        public string level { get; set; }

        public bool live_coverage { get; set; }

        public CoverageDto coverage { get; set; }
    }

    public class CoverageDto
    {
        public bool basic_score { get; set; }

        public bool key_events { get; set; }

        public bool detailed_events { get; set; }

        public bool lineups { get; set; }

        public bool commentary { get; set; }
    }
}