using FakeItEasy;
using Soccer.Core.Leagues.Models;
using Soccer.Database.Leagues.Commands;
using Xunit;

namespace Soccer.Database.Tests.Leagues.Commands
{
    public class InsertLeagueSeasonCommandTests
    {
        private readonly InsertLeagueSeasonCommand command;

        public InsertLeagueSeasonCommandTests()
        {
            command = new InsertLeagueSeasonCommand(A.CollectionOfDummy<League>(5));
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
    }
}
