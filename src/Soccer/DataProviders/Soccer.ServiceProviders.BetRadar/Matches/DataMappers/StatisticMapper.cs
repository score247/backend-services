namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using Soccer.Core.Teams.Models;
    using Soccer.DataProviders.SportRadar.Teams.Dtos;

    public static class StatisticMapper
    {
        public static TeamStatistics MapStatistic(Statistics statisticsDto)
        {
            return new TeamStatistics
            {
                Possession = statisticsDto.ball_possession,
                FreeKicks = statisticsDto.free_kicks,
                ThrowIns = statisticsDto.throw_ins,
                GoalKicks = statisticsDto.goal_kicks,
                ShotsBlocked = statisticsDto.shots_blocked,
                ShotsOnTarget = statisticsDto.shots_on_target,
                ShotsOffTarget = statisticsDto.shots_off_target,
                CornerKicks = statisticsDto.corner_kicks,
                Fouls = statisticsDto.fouls,
                ShotsSaved = statisticsDto.shots_saved,
                Offsides = statisticsDto.offsides,
                YellowCards = statisticsDto.yellow_cards,
                Injuries = statisticsDto.injuries,
                RedCards = statisticsDto.red_cards,
                YellowRedCards = statisticsDto.yellow_red_cards
            };
        }
    }
}
