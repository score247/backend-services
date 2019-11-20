using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Matches.DataMappers;
using Soccer.DataProviders.SportRadar.Matches.Dtos;
using Soccer.DataProviders.SportRadar.Teams.Dtos;

namespace Soccer.DataProviders.SportRadar.Teams.DataMappers
{
    public static class HeadToHeadMapper
    {
        public static IReadOnlyList<Match> MapHeadToHeads(HeadToHeadsDto headToHeadsDto, string region)
        {
            var teamHeadToHeads = new List<Match>();
            var matchTeams = headToHeadsDto?.teams?.ToArray();

            if (matchTeams == null || matchTeams.Length == 0)
            {
                return teamHeadToHeads;
            }

            teamHeadToHeads.AddRange(MapTeamResults(headToHeadsDto.last_meetings?.results, region));
            teamHeadToHeads.AddRange(MapTeamSchedules(headToHeadsDto.next_meetings?.Select(m => m.sport_event), region));

            return teamHeadToHeads;
        }

        private static IReadOnlyList<Match> MapTeamSchedules(IEnumerable<SportEventDto> teamSchedules, string region)
        {
            var teamHeadToHeads = new List<Match>();

            if (teamSchedules == null)
            {
                return teamHeadToHeads;
            }

            foreach (var schedule in teamSchedules)
            {
                var match = MatchMapper.MapMatch(
                        schedule,
                        null,
                        null,
                        region,
                        Language.en_US);

                teamHeadToHeads.Add(match);
            }

            return teamHeadToHeads;
        }

        public static IReadOnlyList<Match> MapTeamResults(IEnumerable<ResultDto> teamResults, string region)
        {
            var teamHeadToHeads = new List<Match>();

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
                        region,
                        Language.en_US);

                teamHeadToHeads.Add(match);
            }

            return teamHeadToHeads;
        }
    }
}