namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using Soccer.Core.Teams.Models;
    using Soccer.DataProviders.SportRadar.Teams.Dtos;

    public static class StatisticMapper
    {
        public static TeamStatistic MapStatistic(Statistics statisticsDto)
            => new TeamStatistic(
                statisticsDto.ball_possession,
                statisticsDto.free_kicks,
                statisticsDto.throw_ins,
                statisticsDto.goal_kicks,
                statisticsDto.shots_blocked,
                statisticsDto.shots_on_target,
                statisticsDto.shots_off_target,
                statisticsDto.corner_kicks,
                statisticsDto.fouls,
                statisticsDto.shots_saved,
                statisticsDto.offsides,
                statisticsDto.yellow_cards,
                statisticsDto.injuries,
                statisticsDto.red_cards,
                statisticsDto.yellow_red_cards
            );
    }
}
