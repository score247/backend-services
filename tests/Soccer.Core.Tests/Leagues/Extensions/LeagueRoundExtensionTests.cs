using AutoFixture;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Xunit;

namespace Soccer.Core.Tests.Leagues.Extensions
{
    public class LeagueRoundExtensionTests
    {
        private readonly Fixture fixture;

        public LeagueRoundExtensionTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void HasGroupStage_RoundTypeIsGroupAndGroupIsNull_ReturnFalse(string group)
        {
            var leagueRound = fixture.Create<LeagueRound>()
                .With(round => round.Type, LeagueRoundType.GroupRound)
                .With(round => round.Group, group);

            Assert.False(leagueRound.HasGroupStage());
        }

        [Fact]
        public void HasGroupStage_RoundTypeIsGroupButGroupNotNull_ReturnTrue()
        {
            var leagueRound = fixture.Create<LeagueRound>()
                .With(round => round.Type, LeagueRoundType.GroupRound)
                .With(round => round.Group, "1");

            Assert.True(leagueRound.HasGroupStage());
        }

        [Fact]
        public void HasGroupStage_RoundTypeIsCup_ReturnTrue()
        {
            var leagueRound = fixture.Create<LeagueRound>()
                .With(round => round.Type, LeagueRoundType.CupRound);

            Assert.True(leagueRound.HasGroupStage());
        }

        [Fact]
        public void HasGroupStage_RoundTypeIsQualifier_ReturnTrue()
        {
            var leagueRound = fixture.Create<LeagueRound>()
                .With(round => round.Type, LeagueRoundType.QualifierRound);

            Assert.True(leagueRound.HasGroupStage());
        }

        [Fact]
        public void HasGroupStage_RoundTypeIsPlayOff_ReturnTrue()
        {
            var leagueRound = fixture.Create<LeagueRound>()
                .With(round => round.Type, LeagueRoundType.PlayOffRound);

            Assert.True(leagueRound.HasGroupStage());
        }
    }
}
