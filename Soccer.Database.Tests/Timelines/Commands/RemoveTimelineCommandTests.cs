using System.Linq;
using AutoFixture;
using FakeItEasy;
using Soccer.Core.Matches.Models;
using Soccer.Database.Timelines.Commands;
using Xunit;

namespace Soccer.Database.Tests.Timelines.Commands
{
    public class RemoveTimelineCommandTests
    {
        private readonly RemoveTimelineCommand command;

        public RemoveTimelineCommandTests()
        {
            var fixture = new Fixture();
            command = new RemoveTimelineCommand(fixture.Create<string>(), A.CollectionOfDummy<TimelineEvent>(5).ToList());
        }

        [Fact]
        public void GetSettingKey_ReturnCorrectCommandName()
        {
            Assert.Equal("Match_RemoveTimelines", command.GetSettingKey());
        }

        [Fact]
        public void IsValid_MatchIdAndTimelinesNotNullOrEmpty_ReturnTrue()
        {
            Assert.True(command.IsValid());
        }
    }
}
