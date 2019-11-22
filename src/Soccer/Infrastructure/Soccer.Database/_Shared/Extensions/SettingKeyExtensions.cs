using System;

namespace Soccer.Database._Shared.Extensions
{
    public static class SettingKeyExtensions
    {
        private const int DateSpan = 3;

        public static string GetCorrespondingKey(this string settingKey, DateTime dateTime)
        {
            if (dateTime < DateTime.Now.AddDays(-DateSpan))
            {
                return $"{settingKey}_Former";
            }

            if (dateTime > DateTime.Now.AddDays(DateSpan))
            {
                return $"{settingKey}_Ahead";
            }

            return settingKey;
        }
    }
}
