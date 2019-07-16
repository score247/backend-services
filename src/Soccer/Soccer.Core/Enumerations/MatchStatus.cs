﻿namespace Soccer.Core.Enumerations
{
    using Soccer.Core.Base;

    public class MatchStatus : Enumeration
    {
        ////not_started – The match is scheduled to be played
        public static readonly MatchStatus NotStarted = new MatchStatus("not_started", nameof(NotStarted));

        ////postponed – The match has been postponed to a future date
        public static readonly MatchStatus Postponed = new MatchStatus("postponed", nameof(Postponed));

        ////start_delayed – The start of the match has been temporarily delayed
        public static readonly MatchStatus StartDelayed = new MatchStatus("start_delayed", nameof(StartDelayed));

        ////cancelled – The match has been canceled and will not be played
        public static readonly MatchStatus Cancelled = new MatchStatus("cancelled", nameof(Cancelled));

        ////live – The match is currently in progress
        public static readonly MatchStatus Live = new MatchStatus("live", nameof(Live));

        ////1st_half – The match is in the first half
        public static readonly MatchStatus FirstHaft = new MatchStatus("1st_half", nameof(FirstHaft));

        ////2nd_half – The match is in the second half
        public static readonly MatchStatus SecondHaft = new MatchStatus("2nd_half", nameof(SecondHaft));

        ////overtime – The match is in overtime
        public static readonly MatchStatus Overtime = new MatchStatus("overtime", nameof(Overtime));

        ////1st_extra – The match is in the first extra period
        public static readonly MatchStatus FirstHaftExtra = new MatchStatus("1st_extra", nameof(FirstHaftExtra));

        ////2nd_extra – The match is in the second extra period
        public static readonly MatchStatus SecondHaftExtra = new MatchStatus("2nd_extra", nameof(SecondHaftExtra));

        ////awaiting_penalties – Waiting for announcement of penalties
        public static readonly MatchStatus AwaitingPenalties = new MatchStatus("awaiting_penalties", nameof(AwaitingPenalties));

        ////penalties – Penalties are ongoing
        public static readonly MatchStatus Penalties = new MatchStatus("penalties", nameof(Penalties));

        ////pause – The match is paused
        public static readonly MatchStatus Pause = new MatchStatus("pause", nameof(Pause));

        ////awaiting_extra_time – Waiting on referee to announce extra time
        public static readonly MatchStatus AwaitingExtraTime = new MatchStatus("awaiting_extra_time", nameof(AwaitingExtraTime));

        ////interrupted – The match has been interrupted
        public static readonly MatchStatus Interrupted = new MatchStatus("interrupted", nameof(Interrupted));

        ////halftime – The match is in halftime
        public static readonly MatchStatus Halftime = new MatchStatus("halftime", nameof(Halftime));

        ////full-time – The match has ended
        public static readonly MatchStatus FullTime = new MatchStatus("full-time", nameof(FullTime));

        ////extra_time – Extra time has been added
        public static readonly MatchStatus ExtraTime = new MatchStatus("extra_time", nameof(ExtraTime));

        ////delayed – The match has been temporarily delayed and will be continued;
        public static readonly MatchStatus Delayed = new MatchStatus("delayed", nameof(Delayed));

        ////abandoned – The match has been abandoned
        public static readonly MatchStatus Abandoned = new MatchStatus("abandoned", nameof(Abandoned));

        ////abandoned – The match has been abandoned
        public static readonly MatchStatus ExtraTimeHaftTime = new MatchStatus("extra_time_halftime", nameof(ExtraTimeHaftTime));

        ////ended – The match is over
        public static readonly MatchStatus Ended = new MatchStatus("ended", nameof(Ended));

        ////closed – The match results have been confirmed
        public static readonly MatchStatus Closed = new MatchStatus("closed", nameof(Closed));

        ////aet – The match has ended after extra time
        public static readonly MatchStatus EndedExtraTime = new MatchStatus("aet", nameof(EndedExtraTime));

        ////ap – The match has ended after penalties
        public static readonly MatchStatus EndedAfterPenalties = new MatchStatus("ap", nameof(EndedAfterPenalties));

        public MatchStatus()
        {
        }

        public MatchStatus(string value, string displayName)
            : base(value?.ToLowerInvariant(), displayName)
        {
        }

        public MatchStatus(string value)
            : base(value?.ToLowerInvariant(), value)
        {
        }

        public bool IsLive() => this == Live;

        public bool IsClosed() => this == Closed;

        public bool IsFirstHalf() => this == FirstHaft;

        public bool IsSecondHalf() => this == SecondHaft;

        public bool IsHalfTime() => this == Halftime;

        public bool IsFirstHaftExtra() => this == FirstHaftExtra;

        public bool IsSecondHalfExtra() => this == SecondHaftExtra;

        public bool IsNotStartOrDelay() => this == NotStarted || this == Postponed || this == Cancelled || this == StartDelayed;
    }
}