using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Matches.DataMappers;
using Soccer.DataProviders.SportRadar.Matches.Dtos;
using Soccer.DataProviders.SportRadar.Teams.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.DataMappers
{
    public static class TeamHeadToHeadMapper
    {
        public static IReadOnlyList<TeamHeadToHead> MapHeadToHeads(TeamHeadToHeadsDto teamHeadToHeadsDto, string region)
        {
            var teamHeadToHeads = new List<TeamHeadToHead>();
            var matchTeams = teamHeadToHeadsDto?.teams?.ToArray();

            if (matchTeams == null || matchTeams.Length == 0)
            {
                return teamHeadToHeads;
            }

            var homeTeamId = matchTeams[0].id;
            var awayTeamId = matchTeams[1].id;

            teamHeadToHeads.AddRange(MapTeamResults(teamHeadToHeadsDto.last_meetings.results, region, homeTeamId, awayTeamId));
            teamHeadToHeads.AddRange(MapTeamSchedules(teamHeadToHeadsDto.next_meetings.Select(m => m.sport_event), region, homeTeamId, awayTeamId));

            return teamHeadToHeads;
        }

        private static IReadOnlyList<TeamHeadToHead> MapTeamSchedules(
            IEnumerable<SportEventDto> teamSchedules, string region, string homeTeamId, string awayTeamId)
        {
            var teamHeadToHeads = new List<TeamHeadToHead>();

            foreach (var schedule in teamSchedules)
            {
                var match = MatchMapper.MapMatch(
                        schedule,
                        null,
                        null,
                        region);

                teamHeadToHeads.Add(MapTeamHeadToHead(homeTeamId, awayTeamId, match));
            }

            return teamHeadToHeads;
        }

        private static IReadOnlyList<TeamHeadToHead> MapTeamResults(
            IEnumerable<ResultDto> teamResults, string region, string homeTeamId, string awayTeamId)
        {
            var teamHeadToHeads = new List<TeamHeadToHead>();

            foreach (var teamResult in teamResults)
            {
                var match = MatchMapper.MapMatch(
                        teamResult.sport_event,
                        teamResult.sport_event_status,
                        null,
                        region);

                teamHeadToHeads.Add(MapTeamHeadToHead(homeTeamId, awayTeamId, match));
            }

            return teamHeadToHeads;
        }

        private static TeamHeadToHead MapTeamHeadToHead(string homeTeamId, string awayTeamId, Match match)
            => new TeamHeadToHead(
                string.IsNullOrWhiteSpace(homeTeamId) ? match?.Teams.FirstOrDefault(t => t.IsHome)?.Id : homeTeamId,
                string.IsNullOrWhiteSpace(awayTeamId) ? match?.Teams.FirstOrDefault(t => !t.IsHome)?.Id : awayTeamId,
                match);
    }
}