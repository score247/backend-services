namespace Soccer.DataProviders.SportRadar.Shared.Extensions
{
    using System.Collections.Generic;
    using Soccer.Core.Shared.Enumerations;

    public static class LanguageExtension
    {
        private static readonly Dictionary<string, string> LanguageMapping = new Dictionary<string, string>
        {
            { Language.English, "en" }
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