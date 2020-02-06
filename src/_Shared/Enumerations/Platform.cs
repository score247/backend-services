namespace Score247.Shared.Enumerations
{
    public class Platform: Enumeration
    {
        public const byte iOS = 1;
        public const byte Android = 2;

        public static readonly Platform IOSPlatform = new Platform(iOS, "iOS");
        public static readonly Platform AndroidPlatform = new Platform(Android, "Android");

        public Platform()
        {
        }

        private Platform(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
