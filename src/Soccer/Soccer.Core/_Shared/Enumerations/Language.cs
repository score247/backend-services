namespace Soccer.Core.Shared.Enumerations
{
    using Score247.Shared.Enumerations;

    public class Language : Enumeration
    {
        public static readonly Language en_US = new Language(1, "en-US");

        public Language()
        {
        }

        private Language(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}