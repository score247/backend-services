﻿namespace Soccer.API.Shared.Middlewares
{
    using System.Collections.Generic;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Sentry;

    public static class ErrorHandlingMiddleware
    {
        private const int InternalErrorServerCode = 500;

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
                    var customInfo = new Dictionary<string, object>();

                    if (exception is DbUpdateConcurrencyException)
                    {
                        var entries = (exception as DbUpdateConcurrencyException).Entries;

                        foreach (var entry in entries)
                        {
                            customInfo.Add("Conflict Item", entry.Metadata.Name);
                        }
                    }

                    await ExceptionHandler.HandleAsync(exception, context, customInfo);

                    context.Response.StatusCode = InternalErrorServerCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(exception?.Message);
                });
            });
        }
    }
}