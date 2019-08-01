namespace Soccer.Core.Matches.Extensions
{
    using Soccer.Core.Matches.Models;

    public static class TimelineExtensions
    {
        private const string NotTakenYetStatus = "not_taken_yet";

        public static bool IsScoreChangeInPenalty(this TimelineEventEntity timeline)
            => timeline.PeriodType != null
                && timeline.PeriodType.IsPenalties
                && timeline.Type.IsScoreChange;

        public static bool IsShootOutInPenalty(this TimelineEventEntity timeline)
            => timeline.PeriodType != null
                && timeline.PeriodType.IsPenalties
                && timeline.Type.IsPenaltyShootout
                && timeline.PenaltyStatus != NotTakenYetStatus;
    }
}