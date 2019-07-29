namespace Soccer.DataProviders.SportRadar._Shared.Extensions
{
    using Soccer.Core._Shared.Enumerations;
    using System.Collections.Generic;

    public static class LanguageExtension
    {
        private static readonly Dictionary<string, string> LanguageMapping = new Dictionary<string, string>
        {
            { "en-US", "en" }
        };

        public static string ToSportRadarFormat(this Language language)
        {
            if (!LanguageMapping.ContainsKey(language.DisplayName))
            {
                throw new KeyNotFoundException($"Missing sportradar definition for {language.DisplayName}");
            }

            return LanguageMapping[language.DisplayName];
        }
    }
}
