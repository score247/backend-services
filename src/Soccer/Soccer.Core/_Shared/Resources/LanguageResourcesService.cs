using System.Globalization;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core._Shared.Resources
{
    public interface ILanguageResourcesService
    {
        string GetString(string value, string language = Language.English);
    }
    public class LanguageResourcesService : ILanguageResourcesService
    {
        public string GetString(string value, string language = Language.English)
        {
            return AppResources.ResourceManager.GetString(value, CultureInfo.GetCultureInfo(language));
        }
    }
}
