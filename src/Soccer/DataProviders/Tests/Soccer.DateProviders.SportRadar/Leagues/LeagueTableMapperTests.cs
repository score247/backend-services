using System.Collections.Generic;
using FakeItEasy;
using Score247.Shared.Tests;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Leagues.Dtos;
using Xunit;

namespace Soccer.DateProviders.SportRadar.Leagues
{
    public class LeagueTableMapperTests
    {
        [Fact]
        public void MapGroupLog_NullTeamLogDto_Null()
        {
            // Arrnage
            TeamLogDto teamLogDto = null;

            // Act
            var groupLog = LeagueTableMapper.MapGroupLog(teamLogDto);

            // Assert
            Assert.Null(groupLog);
        }

        [Fact]
        public void MapGroupLog_TeamLogDtoNullComment_GroupLogNullComment()
        {
            // Arrnage
            var teamLogDto = A.Dummy<TeamLogDto>()
                .With(tl => tl.comments, null);

            // Act
            var groupLog = LeagueTableMapper.MapGroupLog(teamLogDto);

            // Assert
            Assert.Null(groupLog.Comments);
        }

        [Fact]
        public void MapGroupLog_TeamLogDtoEmptyComment_GroupLogEmptyComment()
        {
            // Arrnage
            IEnumerable<LogCommentDto> comments = new List<LogCommentDto>();
            var teamLogDto = A.Dummy<TeamLogDto>()
                .With(tl => tl.comments, comments);

            // Act
            var groupLog = LeagueTableMapper.MapGroupLog(teamLogDto);

            // Assert
            Assert.Empty(groupLog.Comments);
        }
    }
}