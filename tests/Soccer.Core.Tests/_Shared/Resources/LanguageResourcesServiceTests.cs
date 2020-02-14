using Soccer.Core._Shared.Resources;
using Xunit;

namespace Soccer.Core.Tests._Shared.Resources
{
    public class LanguageResourcesServiceTests
    {
        private readonly LanguageResourcesService languageResourcesService;

        public LanguageResourcesServiceTests()
        {
            languageResourcesService = new LanguageResourcesService();
        }

        [Fact]
        public void GetString_InValidCulture_ReturnDefaultValue()
        {
            // Act
            var result = languageResourcesService.GetString("KO", "ou");

            // Assert
            Assert.Equal("KO", result);
        }

        [Fact]
        public void GetString_ValidCulture_ReturnValue()
        {
            // Act
            var result = languageResourcesService.GetString("OneXTwo", "en-US");

            // Assert
            Assert.Equal("1x2", result);
        }

        [Fact]
        public void GetString_InvalidValue_ReturnNull()
        {
            // Act
            var result = languageResourcesService.GetString("Nope", "en-US");

            // Assert
            Assert.Null(result);
        }
    }
}