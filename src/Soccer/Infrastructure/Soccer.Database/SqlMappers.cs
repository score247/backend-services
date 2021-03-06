namespace Soccer.Database
{
    using Dapper;
    using Score247.Shared.Extensions;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Teams.Models;

    public static class SqlMappers
    {
        public static void RegisterJsonTypeHandlers()
        {
            SqlMapper.AddTypeHandler(typeof(Match), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(BetTypeOdds), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(TimelineEvent), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(TeamStatistic), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(MatchLineups), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(LeagueTable), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(LeagueSeasonDates), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(LeagueGroupStage), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(TeamProfile), new JsonTypeHandler());
        }
    }
}