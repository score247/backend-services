namespace Soccer.API.Shared.Middlewares
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    public static class SwaggerMiddleware
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Version = "1.0", Title = "Score247 Query API" });
#pragma warning disable S3902 // "Assembly.GetExecutingAssembly" should not be called
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
#pragma warning restore S3902 // "Assembly.GetExecutingAssembly" should not be called
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void ConfigureSwagger(this IApplicationBuilder application)
        {
            application
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"../swagger/v1/swagger.json", "Score247 Query API Docs");
                });
        }
    }
}