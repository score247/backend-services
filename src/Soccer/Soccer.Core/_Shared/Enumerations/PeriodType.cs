﻿namespace Soccer.Core.Shared.Enumerations
{
    using MessagePack;
    using Score247.Shared.Enumerations;

    [MessagePackObject(keyAsPropertyName: true)]
    public class PeriodType : Enumeration
    {
        public static readonly PeriodType RegularPeriod = new PeriodType(1, "regular_period");
        public static readonly PeriodType Overtime = new PeriodType(2, "overtime");
        public static readonly PeriodType Penalties = new PeriodType(3, "penalties");
        public static readonly PeriodType Pause = new PeriodType(4, "pause");
        public static readonly PeriodType AwaitingExtraTime = new PeriodType(5, "awaiting_extra");
        public static readonly PeriodType ExtraTimeHalfTime = new PeriodType(6, "extra_time_halftime");
        public static readonly PeriodType AwaitingPenalties = new PeriodType(7, "awaiting_penalties");
        public static readonly PeriodType Interrupted = new PeriodType(8, "interrupted");

        public PeriodType()
        {
        }

        public PeriodType(byte value, string displayName)
            : base(value, displayName)
        {
        }

        public PeriodType(byte value)
            : base(value, value.ToString())
        {
        }

        public bool IsPenalties() => this == Penalties;

        public bool IsRegular() => this == RegularPeriod;

        public bool IsOvertime() => this == Overtime;
    }
}