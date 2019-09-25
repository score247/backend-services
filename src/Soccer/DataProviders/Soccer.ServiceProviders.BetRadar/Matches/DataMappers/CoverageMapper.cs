namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public static class CoverageMapper
    {
        public static Coverage MapCoverage(CoverageInfoDto coverageDto)
        {
            return coverageDto.coverage == null
                ? new Coverage(live: coverageDto.live_coverage)
                : new Coverage(
                    live: coverageDto.live_coverage,
                    basicScore: coverageDto.coverage.basic_score,
                    keyEvents: coverageDto.coverage.key_events,
                    detailedEvents: coverageDto.coverage.detailed_events,
                    lineups: coverageDto.coverage.lineups,
                    commentary: coverageDto.coverage.commentary);
        }
    }
}
