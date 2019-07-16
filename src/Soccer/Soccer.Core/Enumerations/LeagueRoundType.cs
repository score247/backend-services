namespace Soccer.Core.Enumerations
{
    using Soccer.Core.Base;

    public class LeagueRoundType : Enumeration
    {
        public const string Cup = "cup";
        public const string Group = "group";
        public const string PlayOff = "playoff";
        public const string Qualifier = "qualification";
        public const string Variable = "variable";

        public static readonly LeagueRoundType CupRound = new LeagueRoundType(Cup, nameof(Cup));
        public static readonly LeagueRoundType GroupRound = new LeagueRoundType(Group, nameof(Group));
        public static readonly LeagueRoundType PlayOffRound = new LeagueRoundType(PlayOff, nameof(PlayOff));
        public static readonly LeagueRoundType QualifierRound = new LeagueRoundType(Qualifier, nameof(Qualifier));
        public static readonly LeagueRoundType VariableRound = new LeagueRoundType(Variable, nameof(Variable));

        public LeagueRoundType()
        {
        }

        public LeagueRoundType(string value, string displayName)
            : base(value, displayName)
        {
        }
    }
}