using System;

namespace Soccer.Database._Shared.Extensions
{
    public static class SettingKeyExtension
    {
        private const int DateSpan = 3;

        public static string GetCorrespondingKey(this string settingKey, DateTimeOffset dateTime, DateTimeOffset currentDate)
        {
            if (dateTime == default)
            {
                return settingKey;
            }

            if (dateTime.ToUniversalTime().Date < currentDate.ToUniversalTime().AddDays(-DateSpan).Date)
            {
                return $"{settingKey}_Former";
            }

            if (dateTime.ToUniversalTime().Date > currentDate.ToUniversalTime().AddDays(DateSpan).Date)
            {
                return $"{settingKey}_Ahead";
            }

            return settingKey;
        }
    }
}
