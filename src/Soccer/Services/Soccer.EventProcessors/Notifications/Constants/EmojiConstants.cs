namespace Soccer.EventProcessors.Notifications.Constants
{
    public static class EmojiConstants
    {
        public const int SOCCER_BALL_ICON = 0x26BD;
        public const int SOUND_ICON = 0x1F50A; 
        public const int FLAG_ICON = 0x1F3C1;
        public const int TROPHY_ICON = 0x1F3C6;

        public static string ConvertIcon(int icon) => char.ConvertFromUtf32(icon);
    }
}
