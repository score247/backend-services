namespace Score247.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperFirstCharInvariant(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var chars = str.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            
            return new string(chars);
        }
    }
}
