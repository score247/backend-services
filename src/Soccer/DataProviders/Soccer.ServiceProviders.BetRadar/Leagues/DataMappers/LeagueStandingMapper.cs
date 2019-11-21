using System.Linq;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;

namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    public static class LeagueStandingMapper
    {
        public static LeagueStanding MapLeagueStanding(TournamentStandingDto tournamentStandingDto, string region)
        {
            var standings = tournamentStandingDto.standings.Select(MapStanding);
            var notes = tournamentStandingDto.notes.Select(MapNote);
            var leagueStanding = new LeagueStanding(
                    LeagueMapper.MapLeague(tournamentStandingDto.tournament, region),
                    LeagueMapper.MapLeagueSeason(tournamentStandingDto.season),
                    standings,
                    notes
                );

            return leagueStanding;
        }

        public static Standing MapStanding(StandingDto standingDto)
        {
            var groupStandings = standingDto.groups.Select(MapGroupStanding);
            var standing = new Standing(
                standingDto.tie_break_rule,
                standingDto.type,
                groupStandings);

            return standing;
        }

        public static GroupStanding MapGroupStanding(GroupStandingDto groupStandingDto)
        {
            var teamStandings = groupStandingDto.team_standings.Select(MapTeamStanding);
            var groupStanding = new GroupStanding(
                groupStandingDto.id,
                groupStandingDto.name,
                teamStandings);

            return groupStanding;
        }

        public static TeamStanding MapTeamStanding(TeamStandingDto teamStandingDto)
        {
            var teamStanding = new TeamStanding(
                teamStandingDto.team.id,
                teamStandingDto.team.name,
                teamStandingDto.rank,
                teamStandingDto.current_outcome,
                teamStandingDto.played,
                teamStandingDto.win,
                teamStandingDto.draw,
                teamStandingDto.loss,
                teamStandingDto.goals_for,
                teamStandingDto.goals_against,
                teamStandingDto.goal_diff,
                teamStandingDto.points,
                teamStandingDto.change
                );

            return teamStanding;
        }

        public static TeamLog MapTeamLog(TeamLogDto teamLogDto)
        {
            var comments = teamLogDto.comments.Select(comment => new Commentary(comment.text));
            var teamNote = new TeamLog(
                teamLogDto.id,
                teamLogDto.name,
                comments
                );

            return teamNote;
        }

        public static GroupLog MapGroupLog(GroupLogDto groupLogDto)
        {
            var teamNotes = groupLogDto.teams.Select(MapTeamLog);
            var groupNote = new GroupLog(
                groupLogDto.id,
                teamNotes);

            return groupNote;
        }

        public static Note MapNote(NoteDto noteDto)
        {
            var note = new Note(MapGroupLog(noteDto.group));
            return note;
        }
    }
}