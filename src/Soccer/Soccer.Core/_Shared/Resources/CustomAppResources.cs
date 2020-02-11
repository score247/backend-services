using System.Globalization;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core._Shared.Resources
{
    public static class CustomAppResources
    {
        public static string GetString(string value, string language = Language.English)
            => AppResources.ResourceManager.GetString(value, CultureInfo.GetCultureInfo(language));
    }
}