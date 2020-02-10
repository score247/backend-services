using System.Globalization;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core._Shared.Resources
{
    public static class CustomAppResources
    {
        public static string GetString(string value, string language = Language.English)
        {
            var cultureResource = AppResources.ResourceManager.GetResourceSet(CultureInfo.GetCultureInfo(language), false, true);
            return cultureResource.GetString(value);
        }
    }
}