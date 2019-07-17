namespace Soccer.Core.Enumerations
{
    using Score247.Shared.Enumerations;

    public class Language : Enumeration
    {
        public static readonly Language en_US = new Language("en-US", nameof(en_US));

        public Language()
        {
        }

        private Language(string value, string displayName)
            : base(value, displayName)
        {
        }
    }
}