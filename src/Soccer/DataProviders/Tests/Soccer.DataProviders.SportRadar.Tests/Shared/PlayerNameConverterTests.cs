using Soccer.DataProviders.SportRadar._Shared;
using Xunit;

namespace Soccer.DataProviders.SportRadar.Shared
{
    public class PlayerNameConverterTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("Player")]
        public void Convert_PlayerIsNullOrNotContainsComma_ReturnInputName(string name)
        {
            var formattedName = PlayerNameConverter.Convert(name);

            Assert.Equal(name, formattedName);
        }

        [Theory]
        [InlineData("Merkulov, Mikhail", "M. Merkulov")]
        [InlineData("Cong Phuong, Nguyen", "N. Cong Phuong")]
        public void Convert_ContainsComma_ReturnCorrectFormat(string name, string expected)
        {
            var formattedName = PlayerNameConverter.Convert(name);

            Assert.Equal(expected, formattedName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("Player")]
        public void ConvertToFullName_PlayerIsNullOrNotContainsComma_ReturnInputName(string name)
        {
            var formattedName = PlayerNameConverter.Convert(name, false);

            Assert.Equal(name, formattedName);
        }

        [Theory]
        [InlineData("Merkulov, Mikhail", "Mikhail Merkulov")]
        [InlineData("Cong Phuong, Nguyen", "Nguyen Cong Phuong")]
        public void ConvertToFullName_ContainsComma_ReturnCorrectFormat(string name, string expected)
        {
            var formattedName = PlayerNameConverter.Convert(name, false);

            Assert.Equal(expected, formattedName);
        }
    }
}
