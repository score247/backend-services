namespace Soccer.EventProcessors.Shared.Middlewares
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.Database;

    public static class DatabaseMiddleware
    {
        public static void AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
        }

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
                .WithMySql(resourcePath: configuration["AppDataPath"])
                .Run();

            SqlMappers.RegisterJsonTypeHandlers();
        }
    }
}