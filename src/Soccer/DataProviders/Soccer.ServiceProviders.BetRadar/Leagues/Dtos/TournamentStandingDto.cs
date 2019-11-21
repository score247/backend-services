namespace Soccer.DataProviders.SportRadar.Leagues.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public class TournamentStandingDto
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public TournamentDto tournament { get; set; }
        public SeasonDto season { get; set; }

        public IEnumerable<StandingDto> standings { get; set; }

        public IEnumerable<NoteDto> notes { get; set; }
    }

    public class StandingDto
    {
        public string tie_break_rule { get; set; }
        public string type { get; set; }

        public IEnumerable<GroupStandingDto> groups { get; set; }
    }

    public class GroupStandingDto
    {
        public string id { get; set; }

        public string name { get; set; }

        public IEnumerable<TeamStandingDto> team_standings { get; set; }
    }

    public class TeamStandingDto
    {
        public CompetitorDto team { get; set; }
        public int rank { get; set; }
        public string current_outcome { get; set; }

        public int played { get; set; }
        public int win { get; set; }
        public int draw { get; set; }
        public int loss { get; set; }
        public int goals_for { get; set; }
        public int goals_against { get; set; }
        public int goal_diff { get; set; }
        public int points { get; set; }
        public int change { get; set; }
    }

    public class NoteDto
    {
        public GroupLogDto group { get; set; }
    }

    public class GroupLogDto
    {
        public string id { get; set; }
        public IEnumerable<TeamLogDto> teams { get; set; }
    }

    public class TeamLogDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public IEnumerable<LogCommentDto> comments { get; set; }
    }

    public class LogCommentDto
    {
        public string text { get; set; }
    }
}