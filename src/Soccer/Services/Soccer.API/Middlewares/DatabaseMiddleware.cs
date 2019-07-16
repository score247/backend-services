namespace Soccer.API.Middlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Soccer.API.Configurations;
    using Soccer.Core.Domain;

    public static class DataBaseMiddleware
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();

            services.AddEntityFrameworkNpgsql()
                   .AddDbContext<SoccerContext>(options
                        => options
                            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                            .EnableSensitiveDataLogging()
                            .UseLoggerFactory(loggerFactory))
                   .BuildServiceProvider();
        }

        public static void UseDatabase(this IApplicationBuilder app)
        {
            var appSettings = app.ApplicationServices.GetService<IAppSettings>();

            if (appSettings.EnabledDatabaseMigration)
            {
                using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<SoccerContext>().Database.Migrate();
                }
            }
        }
    }
}