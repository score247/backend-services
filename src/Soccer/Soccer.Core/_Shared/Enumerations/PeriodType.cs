namespace Soccer.Core.Enumerations
{
    using Score247.Shared.Enumerations;

    public class PeriodType : Enumeration
    {
        public static readonly PeriodType RegularPeriod = new PeriodType(1, "regular_period");
        public static readonly PeriodType Overtime = new PeriodType(2, "overtime");
        public static readonly PeriodType Penalties = new PeriodType(3, "penalties");

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
    }
}