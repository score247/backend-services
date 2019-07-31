namespace Soccer.API.Shared.Middlewares
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;

    public static class DataBaseMiddleware
    {
        public static void UseDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            Dictionary<string, ConnectionConfiguration> connections = configuration
                 .GetSection("ConnectionStrings")
                 .GetChildren()
                 .ToDictionary(connection => connection.Key,
                               connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                .WithMySql(resourcePath: configuration["AppDataPath"]) // It comes with a default connection string provider, which works well with MySql connections, as well as a default DbSetting provider
                .Run();
        }
    }
}