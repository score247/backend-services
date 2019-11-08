namespace Soccer.Core.Shared.Enumerations
{
    using Score247.Shared.Enumerations;

    public class Language : Enumeration
    {
        public const string English = "en-US";

        public static readonly Language en_US = new Language(1, English);

        public Language()
        {
        }

        private Language(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}