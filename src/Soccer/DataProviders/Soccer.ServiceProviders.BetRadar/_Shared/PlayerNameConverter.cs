using System;

namespace Soccer.DataProviders.SportRadar._Shared
{
    public static class PlayerNameConverter
    {
        private const string comma = ",";

        public static string Convert(string playerName)
        {
            if (!string.IsNullOrWhiteSpace(playerName) && playerName.Contains(comma))
            {
                var firstCommaIndex = playerName.IndexOf(comma, StringComparison.OrdinalIgnoreCase);
                var minAllowedLength = firstCommaIndex + 2;
                if (firstCommaIndex > 0 && playerName.Length > minAllowedLength)
                {
                    var firstName = playerName.Substring(firstCommaIndex + 2).ToUpperInvariant();
                    var lastName = playerName.Substring(0, firstCommaIndex);

                    return $"{firstName[0]}. {lastName}";
                }
            }

            return playerName;
        }
    }
}