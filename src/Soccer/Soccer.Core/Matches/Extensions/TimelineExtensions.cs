namespace Soccer.Core.Matches.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public static class TimelineExtensions
    {
        public static List<byte> BasicEventTypes { get; }
           = new List<byte>
           {
                EventType.MatchStarted.Value,
                EventType.PeriodStart.Value,
                EventType.BreakStart.Value,
                EventType.ScoreChange.Value,
                EventType.YellowCard.Value,
                EventType.YellowRedCard.Value,
                EventType.RedCard.Value,
                EventType.PenaltyShootout.Value,
                EventType.MatchEnded.Value,
                EventType.InjuryTimeShown.Value,
                EventType.PenaltyMissed.Value,
                EventType.Substitution.Value
           };

        private static readonly EventType[] MainEventTypes = {
            EventType.ScoreChange,
            EventType.PenaltyMissed,
            EventType.YellowCard,
            EventType.RedCard,
            EventType.YellowRedCard,
            EventType.PenaltyShootout,
            EventType.BreakStart,
            EventType.MatchEnded,
            EventType.Substitution
        };

        private static readonly EventType[] NotifiableEventTypes = {
            EventType.MatchStarted,
            EventType.ScoreChange,
            EventType.RedCard,
            EventType.YellowRedCard,
            EventType.MatchEnded
        };

        public static List<byte> ReprocessScoreEventTypes { get; }
           = new List<byte>
           {
                EventType.BreakStart.Value,
                EventType.MatchEnded.Value,
                EventType.PenaltyMissed.Value
           };

        private const string NotTakenYetStatus = "not_taken_yet";

        public static bool IsScoreChangeInPenalty(this TimelineEvent timeline)
            => timeline.PeriodType != null
                && timeline.PeriodType.IsPenalties()
                && timeline.Type.IsScoreChange();

        public static bool IsShootOutInPenalty(this TimelineEvent timeline)
            => timeline.PeriodType != null
                && timeline.PeriodType.IsPenalties()
                && timeline.Type.IsPenaltyShootout()
                && timeline.PenaltyStatus?.ToLowerInvariant() != NotTakenYetStatus;

        public static bool IsBasicEvent(this TimelineEvent timeline)
            => BasicEventTypes.Contains(timeline.Type.Value);

        public static bool ShouldReprocessScore(this TimelineEvent timeline)
            => ReprocessScoreEventTypes.Contains(timeline.Type.Value);

        public static bool IsMainEvent(this EventType eventType)
            => MainEventTypes.Contains(eventType);

        public static bool IsNotifableEvent(this EventType eventType)
            => NotifiableEventTypes.Contains(eventType);
    }
}