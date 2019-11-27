using System;

namespace Soccer.Database._Shared.Extensions
{
    public static class SettingKeyExtensions
    {
        private const int DateSpan = 3;

        public static string GetCorrespondingKey(this string settingKey, DateTimeOffset dateTime)
        {
            if (dateTime < DateTimeOffset.Now.AddDays(-DateSpan))
            {
                return $"{settingKey}_Former";
            }

            if (dateTime > DateTimeOffset.Now.AddDays(DateSpan))
            {
                return $"{settingKey}_Ahead";
            }

            return settingKey;
        }
    }
}
