namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public static class CoverageMapper
    {
        public static Coverage MapCoverage(CoverageInfoDto coverageDto, string widgetLink)
        {
            return coverageDto.coverage == null
                ? new Coverage(coverageDto.live_coverage, widgetLink)
                : new Coverage(
                    live: coverageDto.live_coverage,
                    widgetLink,
                    basicScore: coverageDto.coverage.basic_score,
                    keyEvents: coverageDto.coverage.key_events,
                    detailedEvents: coverageDto.coverage.detailed_events,
                    lineups: coverageDto.coverage.lineups,
                    commentary: coverageDto.coverage.commentary);
        }
    }
}
