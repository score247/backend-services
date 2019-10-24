using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Matches.DataMappers;
using Soccer.DataProviders.SportRadar.Matches.Dtos;
using Soccer.DataProviders.SportRadar.Teams.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.DataMappers
{
    public static class HeadToHeadMapper
    {
        public static IReadOnlyList<HeadToHead> MapHeadToHeads(HeadToHeadsDto headToHeadsDto, string region)
        {
            var teamHeadToHeads = new List<HeadToHead>();
            var matchTeams = headToHeadsDto?.teams?.ToArray();

            if (matchTeams == null || matchTeams.Length == 0)
            {
                return teamHeadToHeads;
            }

            var homeTeamId = matchTeams[0].id;
            var awayTeamId = matchTeams[1].id;

            teamHeadToHeads.AddRange(MapTeamResults(headToHeadsDto.last_meetings?.results, region, homeTeamId, awayTeamId));
            teamHeadToHeads.AddRange(MapTeamSchedules(headToHeadsDto.next_meetings?.Select(m => m.sport_event), region, homeTeamId, awayTeamId));

            return teamHeadToHeads;
        }

        private static IReadOnlyList<HeadToHead> MapTeamSchedules(
            IEnumerable<SportEventDto> teamSchedules, string region, string homeTeamId, string awayTeamId)
        {
            var teamHeadToHeads = new List<HeadToHead>();

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

        private static IReadOnlyList<HeadToHead> MapTeamResults(
            IEnumerable<ResultDto> teamResults, string region, string homeTeamId, string awayTeamId)
        {
            var teamHeadToHeads = new List<HeadToHead>();

            if (teamResults == null)
            {
                return teamHeadToHeads;
            }

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

        private static HeadToHead MapTeamHeadToHead(string homeTeamId, string awayTeamId, Match match)
            => new HeadToHead(
                string.IsNullOrWhiteSpace(homeTeamId) ? match?.Teams.FirstOrDefault(t => t.IsHome)?.Id : homeTeamId,
                string.IsNullOrWhiteSpace(awayTeamId) ? match?.Teams.FirstOrDefault(t => !t.IsHome)?.Id : awayTeamId,
                match);
    }
}