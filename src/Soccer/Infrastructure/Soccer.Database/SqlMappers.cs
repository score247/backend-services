namespace Soccer.Database
{
    using Dapper;
    using Score247.Shared.Extensions;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Matches.Models;

    public static class SqlMappers
    {
        public static void RegisterJsonTypeHandlers()
        {
            SqlMapper.AddTypeHandler(typeof(Match), new JsonTypeHandler());
            SqlMapper.AddTypeHandler(typeof(League), new JsonTypeHandler());
        }
    }
}