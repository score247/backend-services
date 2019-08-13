namespace Soccer.EventProcessors.Shared.Middlewares
{
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Sentry;

    public static class ErrorHandlingMiddleware
    {
        public static void AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            LogManager
                   .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
                   .Use(new SentryLogging(new SentryEngineOptions
                   {
                       Dsn = new Dsn(configuration["Fanex.Logging:SentryUrl"])
                   }));

            services.AddSingleton(Logger.Log);
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    var logger = app.ApplicationServices.GetRequiredService<ILogger>();
                    await logger.ErrorAsync(exception?.Message, exception);
                    await context.Response.WriteAsync(exception?.Message);
                });
            });
        }
    }
}