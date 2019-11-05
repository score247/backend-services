using System;

namespace Soccer.DataProviders.SportRadar._Shared
{
    public static class PlayerNameConverter
    {
        private const string comma = ",";

        public static string Convert(string playerName, bool firstNameInShort = true)
        {
            var formattedName = playerName;

            if (!string.IsNullOrWhiteSpace(playerName) && playerName.Contains(comma))
            {
                var firstCommaIndex = playerName.IndexOf(comma, StringComparison.OrdinalIgnoreCase);
                var minAllowedLength = firstCommaIndex + 2;

                if (firstCommaIndex > 0 && playerName.Length > minAllowedLength)
                {
                    var firstName = firstNameInShort
                                    ? playerName.Substring(firstCommaIndex + 2).ToUpperInvariant()
                                    : playerName.Substring(firstCommaIndex + 2);

                    var lastName = playerName.Substring(0, firstCommaIndex);

                    formattedName = firstNameInShort
                        ? $"{firstName[0]}. {lastName}"
                        : $"{firstName} {lastName}";
                }
            }

            return formattedName;
        }
    }
}