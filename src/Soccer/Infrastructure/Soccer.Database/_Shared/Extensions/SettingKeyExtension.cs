using System;
using Score247.Shared.Extensions;

namespace Soccer.Database._Shared.Extensions
{
    public static class SettingKeyExtension
    {
        public static string GetCorrespondingKey(this string settingKey, DateTimeOffset dateTime, DateTimeOffset currentDate)
        {
            if (dateTime == default)
            {
                return settingKey;
            }

            if (dateTime.IsFormer(currentDate))
            {
                return $"{settingKey}_Former";
            }

            if (dateTime.IsAhead(currentDate))
            {
                return $"{settingKey}_Ahead";
            }

            return settingKey;
        }
    }
}