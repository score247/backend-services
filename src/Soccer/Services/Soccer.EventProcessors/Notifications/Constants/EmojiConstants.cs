namespace Soccer.EventProcessors.Notifications.Constants
{
    public static class EmojiConstants
    {
        public const string SOCCER_BALL_ICON = "\u26BD";
        public const int SOUND_ICON = 0x1F50A; 
        public const string FLAG_ICON = "\u1F3C1";
        public const string TROPHY_ICON = "\uF3C6";

        public static string ConvertIcon(int icon) => char.ConvertFromUtf32(icon);
    }
}
