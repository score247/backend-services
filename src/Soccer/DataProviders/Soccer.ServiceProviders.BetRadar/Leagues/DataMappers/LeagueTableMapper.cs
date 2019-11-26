using System.Collections.Generic;
using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;

namespace Soccer.DataProviders.SportRadar.Leagues.DataMappers
{
    public static class LeagueTableMapper
    {
        public static LeagueTable MapLeagueTable(TournamentDto tournamentDto, SeasonDto seasonDto, IEnumerable<NoteDto> noteDtos, StandingDto standingDto, string region)
        {
            var groupTable = standingDto.groups.Select(group => MapGroupTable(group, noteDtos));
            var tableType = Enumeration.FromDisplayName<LeagueTableType>(standingDto.type);
            var leagueTable = new LeagueTable(
                    LeagueMapper.MapLeague(tournamentDto, region),
                    tableType,
                    LeagueMapper.MapLeagueSeason(seasonDto),
                    groupTable);

            return leagueTable;
        }

        public static LeagueGroupTable MapGroupTable(GroupStandingDto groupStandingDto, IEnumerable<NoteDto> noteDtos)
        {
            var groupNotes = new List<LeagueGroupNote>();
            if (noteDtos != null)
            {
                var teamNotes = noteDtos.FirstOrDefault(noteDto => noteDto.group.id == groupStandingDto.id);
                if (teamNotes != null)
                {
                    groupNotes = teamNotes.group.teams.Select(MapGroupLog).ToList();
                }
            }
            var teamStanding = groupStandingDto.team_standings.Select(MapTeamStanding);
            var groupTable = new LeagueGroupTable(
                groupStandingDto.id,
                groupStandingDto.name,
                groupNotes,
                teamStanding);

            return groupTable;
        }

        public static TeamStanding MapTeamStanding(TeamStandingDto teamStandingDto)
        {
            var teamStanding = new TeamStanding(
                teamStandingDto.team.id,
                teamStandingDto.team.name,
                teamStandingDto.rank,
                string.IsNullOrWhiteSpace(teamStandingDto.current_outcome)
                    ? TeamOutcome.Unknown
                    : Enumeration.FromDisplayName<TeamOutcome>(teamStandingDto.current_outcome.ToLowerInvariant()),
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

        public static LeagueGroupNote MapGroupLog(TeamLogDto teamLogDto)
        {
            var groupNote = new LeagueGroupNote(
                teamLogDto.id,
                teamLogDto.name,
                teamLogDto.comments.Select(comment => comment.text)
                );

            return groupNote;
        }
    }
}