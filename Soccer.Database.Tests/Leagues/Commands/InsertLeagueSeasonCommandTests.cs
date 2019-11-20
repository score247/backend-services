using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Leagues.Commands;
using Xunit;

namespace Soccer.Database.Tests.Leagues.Commands
{
    public class InsertLeagueSeasonCommandTests
    {
        private readonly InsertLeagueSeasonCommand command;

        public InsertLeagueSeasonCommandTests() 
        {
            command = new InsertLeagueSeasonCommand(A.CollectionOfDummy<League>(5), Language.en_US.DisplayName);
        }

        [Fact]
        public void GetSettingKey_ReturnCorrectCommandName()
        {
            Assert.Equal("League_InsertLeagueSeason", command.GetSettingKey());
        }

        [Fact]
        public void IsValid_LanguageAndLeaguesNotNullOrEmpty_ReturnTrue()
        {
            Assert.True(command.IsValid());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsValid_LanguageNullOrEmpty_ReturnFalse(string language)
        {
            var invalidCommand = new InsertLeagueSeasonCommand(A.CollectionOfDummy<League>(5), language);

            Assert.False(invalidCommand.IsValid());
        }
    }
}
