using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.Extensions
{
    public static class MatchResultExtension
    {
        private const int NUMBER_OF_PERIODS_IN_MAIN_TIME = 2;

        public static bool IsAfterExtra(this MatchResult matchResult)
           => matchResult.EventStatus.IsEndedOrClosed() &&
            matchResult.MatchPeriods.Any(period => period.PeriodType == PeriodType.Overtime) && 
            !matchResult.MatchPeriods.Any(period => period.PeriodType == PeriodType.Penalties);

        public static bool IsAfterPenaltyShootout(this MatchResult matchResult)
          => matchResult.EventStatus.IsEndedOrClosed() &&
           matchResult.MatchPeriods.Any(period => period.PeriodType == PeriodType.Penalties);

        public static bool IsEndedInMainTime(this MatchResult matchResult)
          => matchResult.EventStatus.IsEndedOrClosed() &&
           matchResult.MatchPeriods?.Count() <= NUMBER_OF_PERIODS_IN_MAIN_TIME;
    }
}
