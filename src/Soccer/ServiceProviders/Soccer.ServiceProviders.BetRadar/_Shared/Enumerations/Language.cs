namespace Soccer.DataProviders.SportRadar._Shared.Enumerations
{
    using Score247.Shared.Enumerations;

    internal class Language : Enumeration
    {
        public static readonly Language English = new Language("en-US", "en");

        public Language()
        {
        }

        private Language(string value, string displayName)
            : base(value, displayName)
        {
        }
    }
}